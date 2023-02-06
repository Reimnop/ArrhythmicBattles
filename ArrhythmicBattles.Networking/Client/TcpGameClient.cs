using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using ArrhythmicBattles.Common;

namespace ArrhythmicBattles.Networking.Client;

public class TcpGameClient : GameClient, IDisposable
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    
    public TcpGameClient(IPAddress address, int port)
    {
        client = new TcpClient();
        client.Connect(address, port);
        stream = client.GetStream();
    }
    
    public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer)
    {
        return stream.WriteAsync(buffer);
    }

    public override async ValueTask<ReadOnlyMemory<byte>> ReceiveAsync()
    {
        await TaskHelper.WaitUntil(() => client.Available > 0, cancellationToken: cancellationTokenSource.Token);

        // I hope the garbage collector doesn't get mad at me for this
        byte[] buffer = new byte[client.Available];
        int bytesRead = await stream.ReadAsync(buffer);
        Debug.Assert(bytesRead == buffer.Length); // Better safe than sorry
        return buffer;
    }

    public override void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        client.Dispose();
    }
}