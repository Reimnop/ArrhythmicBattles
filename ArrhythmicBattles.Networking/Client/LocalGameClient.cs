using System.Collections.Concurrent;
using ArrhythmicBattles.Common;
using ArrhythmicBattles.Networking.Server.Local;

namespace ArrhythmicBattles.Networking.Client;

public class LocalGameClient : GameClient, IDisposable
{
    private readonly ByteQueue byteQueue = new ByteQueue();
    private readonly ServerLocalSocket server;
    
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    
    public LocalGameClient(ServerLocalSocket server)
    {
        this.server = server;
    }
    
    internal ValueTask WriteAsync(ReadOnlyMemory<byte> buffer)
    {
        byteQueue.Enqueue(buffer.ToArray(), 0, buffer.Length);
        return ValueTask.CompletedTask;
    }
    
    public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer)
    {
        return server.WriteAsync(this, buffer);
    }

    public override async ValueTask<ReadOnlyMemory<byte>> ReceiveAsync(int length)
    {
        byte[] buffer = new byte[length == -1 ? byteQueue.Length : length];
        await TaskHelper.WaitUntil(() => byteQueue.Length >= length, cancellationToken: cancellationTokenSource.Token);
        byteQueue.Dequeue(buffer, 0, buffer.Length);
        return buffer;
    }

    public override void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        server.RemoveClient(this);
    }
}