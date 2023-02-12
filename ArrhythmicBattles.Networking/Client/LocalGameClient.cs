using System.Collections.Concurrent;
using ArrhythmicBattles.Common;
using ArrhythmicBattles.Networking.Server.Local;

namespace ArrhythmicBattles.Networking.Client;

public class LocalGameClient : GameClient, IDisposable
{
    private readonly ByteQueue byteQueue = new ByteQueue();
    private readonly ServerLocalSocket server;

    public LocalGameClient(ServerLocalSocket server)
    {
        this.server = server;
    }
    
    internal ValueTask WriteAsync(ReadOnlyMemory<byte> buffer)
    {
        byteQueue.Enqueue(buffer.Span);
        return ValueTask.CompletedTask;
    }
    
    public override async Task SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await server.WriteAsync(this, buffer);
    }

    public override async Task<ReadOnlyMemory<byte>> ReceiveAsync(int length, CancellationToken cancellationToken = default)
    {
        byte[] buffer = new byte[length == -1 ? byteQueue.Length : length];
        await TaskHelper.WaitUntil(() => byteQueue.Length >= length, cancellationToken: cancellationToken);
        byteQueue.Dequeue(buffer);
        return buffer;
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