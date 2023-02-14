using System.Collections.Concurrent;
using ArrhythmicBattles.Common;
using ArrhythmicBattles.Common.Async;
using ArrhythmicBattles.Networking.Packets;

namespace ArrhythmicBattles.Networking;

public class NetworkHandler : IDisposable
{
    private struct EventListener<T>
    {
        public bool IsCompleted => result != null;
        public T? Result => result;
        
        private T? result = default;
        private readonly AsyncEvent<T> asyncEvent;
        
        public EventListener(AsyncEvent<T> asyncEvent)
        {
            this.asyncEvent = asyncEvent;
            
            result = default;
            asyncEvent.AddCallback(OnEvent);
        }

        private Task OnEvent(object sender, T args)
        {
            result = args;
            asyncEvent.RemoveCallback(OnEvent);
            return Task.CompletedTask;
        }
    }
    
    public AsyncEvent<Packet> PacketReceived { get; } = new AsyncEvent<Packet>();

    private readonly TypedPacketTunnel tunnel;
    private readonly ConcurrentQueue<Packet> queuedOutgoingPackets = new ConcurrentQueue<Packet>();
    
    private readonly Dictionary<Type, AsyncCallback<Packet>> packetHandlers = new Dictionary<Type, AsyncCallback<Packet>>();

    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly Task receivePacketLoopTask;
    private readonly Task sendPacketLoopTask;

    public NetworkHandler(ISenderReceiver senderReceiver)
    {
        tunnel = new TypedPacketTunnel(senderReceiver);

        receivePacketLoopTask = Task.Run(ReceivePacketLoopAsync);
        sendPacketLoopTask = Task.Run(SendPacketLoopAsync);
    }
    
    public Exception? GetException()
    {
        if (sendPacketLoopTask.IsFaulted)
        {
            return sendPacketLoopTask.Exception;
        }
        
        if (receivePacketLoopTask.IsFaulted)
        {
            return receivePacketLoopTask.Exception;
        }
        
        return null;
    }
    
    private async Task ReceivePacketLoopAsync()
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Packet? packet = await tunnel.ReceiveAsync();
            if (packet != null)
            {
                await PacketReceived.InvokeAsync(this, packet);
            }
        }
    }

    private async Task SendPacketLoopAsync()
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Packet? packet = null;
            await TaskHelper.WaitUntil(() => queuedOutgoingPackets.TryDequeue(out packet), cancellationToken: cancellationTokenSource.Token);

            if (packet != null)
            {
                using CancellationTokenSource cts = new CancellationTokenSource(20000);
                await tunnel.SendAsync(packet, cts.Token);
            }
        }
    }

    public async Task<T?> ReceivePacketAsync<T>() where T : Packet
    {
        Packet? packet = null;
        do
        {
            EventListener<Packet> listener = new EventListener<Packet>(PacketReceived);
            await TaskHelper.WaitUntil(() => listener.IsCompleted, cancellationToken: cancellationTokenSource.Token);
            packet = listener.Result;
        }
        while (packet != null && packet.GetType() != typeof(T) && !cancellationTokenSource.IsCancellationRequested);
        
        return (T?) packet;
    }

    /// <param name="shouldBlock">If true, the packet will be sent immediately. If false, the packet will be queued and sent in the background.</param>
    public async Task SendPacketAsync(Packet packet, bool shouldBlock = false)
    {
        if (!shouldBlock)
        {
            using CancellationTokenSource cts = new CancellationTokenSource(20000);
            await tunnel.SendAsync(packet, cts.Token);
            return;
        }
        
        queuedOutgoingPackets.Enqueue(packet);
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}