using System.Collections.Concurrent;
using ArrhythmicBattles.Common;
using ArrhythmicBattles.Common.Async;
using ArrhythmicBattles.Networking.Packets;

namespace ArrhythmicBattles.Networking;

public class NetworkHandler : IDisposable
{
    public AsyncEvent<Packet> PacketReceived { get; } = new AsyncEvent<Packet>();

    private readonly TypedPacketTunnel tunnel;
    private readonly PacketCollector collector;
    private readonly ConcurrentQueue<Packet> queuedOutgoingPackets = new ConcurrentQueue<Packet>();
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    
    private readonly Task sendPacketLoopTask;

    public NetworkHandler(ISenderReceiver senderReceiver)
    {
        tunnel = new TypedPacketTunnel(senderReceiver);
        collector = new PacketCollector(tunnel);
        
        sendPacketLoopTask = Task.Run(SendPacketLoopAsync);
        collector.PacketReceived.AddCallback(OnPacketReceived);
    }
    
    public Exception? GetException()
    {
        if (sendPacketLoopTask.IsFaulted)
        {
            return sendPacketLoopTask.Exception;
        }
        
        Exception? collectorException = collector.GetException();
        if (collectorException != null)
        {
            return collectorException;
        }

        return null;
    }
    
    private async Task OnPacketReceived(object sender, Packet packet)
    {
        await PacketReceived.InvokeAsync(this, packet);
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
    
    public void SendPacket(Packet packet)
    {
        queuedOutgoingPackets.Enqueue(packet);
    }

    public Task<T?> ReceivePacketAsync<T>() where T : Packet
    {
        return collector.ReceivePacketAsync<T>();
    }
    
    public Task<T?> GetPacketAsync<T>() where T : Packet
    {
        return collector.GetPacketAsync<T>();
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        collector.Dispose();
    }
}