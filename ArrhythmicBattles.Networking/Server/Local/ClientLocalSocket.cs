using System.Collections.Concurrent;

namespace ArrhythmicBattles.Networking.Server.Local;

public class ClientLocalSocket : ClientSocket, IDisposable
{
    private readonly ConcurrentQueue<ReadOnlyMemory<byte>> queuedPackets = new ConcurrentQueue<ReadOnlyMemory<byte>>(); 
    private readonly ServerLocalSocket server;
    
    public ClientLocalSocket(ServerLocalSocket server)
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

    public override ValueTask<Memory<byte>> ReceiveAsync()
    {
        if (queuedPackets.TryDequeue(out ReadOnlyMemory<byte> packet))
        {
            // Copy the packet to a new buffer
            Memory<byte> buffer = new byte[packet.Length];
            packet.CopyTo(buffer);
            return ValueTask.FromResult(buffer);
        }

        return ValueTask.FromResult(Memory<byte>.Empty);
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