using System.Collections.Concurrent;
using ArrhythmicBattles.Common;
using ArrhythmicBattles.Common.Async;
using ArrhythmicBattles.Networking.Packets;

namespace ArrhythmicBattles.Networking;

public class PacketCollector : IDisposable
{
    public AsyncEvent<Packet> PacketReceived { get; } = new AsyncEvent<Packet>();

    private readonly TypedPacketTunnel tunnel;
    private readonly Dictionary<Type, ConcurrentQueue<Packet>> packetQueues = new Dictionary<Type, ConcurrentQueue<Packet>>();

    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly Task receiveLoopTask;
    
    public PacketCollector(TypedPacketTunnel tunnel)
    {
        this.tunnel = tunnel;
        receiveLoopTask = Task.Run(ReceiveLoop);
    }
    
    public Exception? GetException()
    {
        if (receiveLoopTask.IsFaulted)
        {
            return receiveLoopTask.Exception;
        }

        return null;
    }
    
    private async Task ReceiveLoop()
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Packet? packet = await tunnel.ReceiveAsync();
            if (packet != null)
            {
                if (!packetQueues.TryGetValue(packet.GetType(), out ConcurrentQueue<Packet>? queue))
                {
                    queue = new ConcurrentQueue<Packet>();
                    packetQueues.Add(packet.GetType(), queue);
                }
                
                queue.Enqueue(packet);
                
                await PacketReceived.InvokeAsync(this, packet);
            }
        }
    }
    
    // This waits for a packet of the given type to be received
    public async Task<T?> ReceivePacketAsync<T>(CancellationToken cancellationToken = default) where T : Packet
    {
        Packet? packet = null;
        await TaskHelper.WaitUntil(() => packetQueues.TryGetValue(typeof(T), out ConcurrentQueue<Packet>? queue) && queue.TryDequeue(out packet), cancellationToken: cancellationToken);
        return (T?) packet;
    }
    
    // This does not wait for a packet of the given type to be received
    public Task<T?> GetPacketAsync<T>() where T : Packet
    {
        if (packetQueues.TryGetValue(typeof(T), out ConcurrentQueue<Packet>? queue) && queue.TryDequeue(out Packet? packet))
        {
            return Task.FromResult((T?) packet);
        }
        
        return Task.FromResult((T?) null);
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}