using System.Collections.Concurrent;
using ArrhythmicBattles.Common;

namespace ArrhythmicBattles.Networking.Server.Local;

public class ClientLocalSocket : ClientSocket, IDisposable
{
    private readonly ByteQueue byteQueue = new ByteQueue();
    private readonly ServerLocalSocket server;
    
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public ClientLocalSocket(ServerLocalSocket server)
    {
        this.server = server;
    }

    internal ValueTask WriteAsync(ReadOnlyMemory<byte> buffer)
    {
        byteQueue.Enqueue(buffer.Span);
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
        byteQueue.Dequeue(buffer);
        return buffer;
    }

    public override string GetName()
    {
        return "Local";
    }

    public override void Close()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        // Cancel the receive loop
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        server.RemoveClient(this);
    }
}