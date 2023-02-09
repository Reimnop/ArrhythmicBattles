using System.Collections.Concurrent;
using ArrhythmicBattles.Common;
using ArrhythmicBattles.Networking.Packets;

namespace ArrhythmicBattles.Networking;

public class PacketCollector : IDisposable
{
    private readonly TypedPacketTunnel tunnel;
    private readonly Dictionary<Type, ConcurrentQueue<Packet>> packetQueues = new Dictionary<Type, ConcurrentQueue<Packet>>();
    
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public PacketCollector(TypedPacketTunnel tunnel)
    {
        this.tunnel = tunnel;
        Task.Run(ReceiveLoop);
    }
    
    private async Task ReceiveLoop()
    {
        Packet? packet;
        do
        {
            packet = await tunnel.ReceiveAsync();
            if (packet != null)
            {
                if (!packetQueues.TryGetValue(packet.GetType(), out ConcurrentQueue<Packet>? queue))
                {
                    queue = new ConcurrentQueue<Packet>();
                    packetQueues.Add(packet.GetType(), queue);
                }
                queue.Enqueue(packet);
            }
        }
        while (packet != null);
        
        cancellationTokenSource.Cancel();
    }
    
    // This waits for a packet of the given type to be received
    public async Task<T?> ReceivePacketAsync<T>() where T : Packet
    {
        Packet? packet = null;
        await TaskHelper.WaitUntil(() => packetQueues.TryGetValue(typeof(T), out ConcurrentQueue<Packet>? queue) && queue.TryDequeue(out packet), cancellationToken: cancellationTokenSource.Token);
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