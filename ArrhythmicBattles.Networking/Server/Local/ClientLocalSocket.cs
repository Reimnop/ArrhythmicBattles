using System.Collections.Concurrent;
using ArrhythmicBattles.Common;

namespace ArrhythmicBattles.Networking.Server.Local;

public class ClientLocalSocket : ClientSocket, IDisposable
{
    private readonly ConcurrentQueue<ReadOnlyMemory<byte>> queuedPackets = new ConcurrentQueue<ReadOnlyMemory<byte>>(); 
    private readonly ServerLocalSocket server;
    
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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

    public override async ValueTask<ReadOnlyMemory<byte>> ReceiveAsync()
    {
        ReadOnlyMemory<byte> packet = default;
        await TaskHelper.WaitUntil(() => queuedPackets.TryDequeue(out packet), cancellationToken: cancellationTokenSource.Token);
        return packet;
    }

    public override void Close()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        // Cancel the receive loop
        cancellationTokenSource.Cancel();
        server.RemoveClient(this);
    }
}