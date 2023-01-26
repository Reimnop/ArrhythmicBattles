using System.Collections.Concurrent;
using ArrhythmicBattles.Networking.Server.Local;

namespace ArrhythmicBattles.Networking.Client;

public class LocalGameClient : GameClient, IDisposable
{
    private readonly ConcurrentQueue<ReadOnlyMemory<byte>> queuedPackets = new ConcurrentQueue<ReadOnlyMemory<byte>>(); 
    private readonly ServerLocalSocket server;
    
    public LocalGameClient(ServerLocalSocket server)
    {
        this.server = server;
    }
    
    internal ValueTask WriteAsync(ReadOnlyMemory<byte> buffer)
    {
        queuedPackets.Enqueue(buffer);
        return ValueTask.CompletedTask;
    }
    
    public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer)
    {
        return server.WriteAsync(this, buffer);
    }

    public override ValueTask<ReadOnlyMemory<byte>> ReceiveAsync()
    {
        if (queuedPackets.TryDequeue(out ReadOnlyMemory<byte> packet))
        {
            return ValueTask.FromResult(packet);
        }
        
        return ValueTask.FromResult(ReadOnlyMemory<byte>.Empty);
    }

    public override void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        server.RemoveClient(this);
    }
}