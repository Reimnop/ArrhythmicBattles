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
        client.ReceiveTimeout = 20000;
        client.SendTimeout = 20000;
        client.NoDelay = true;
        client.Connect(address, port);
        stream = client.GetStream();
    }
    
    public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer)
    {
        return stream.WriteAsync(buffer);
    }

    public override async ValueTask<ReadOnlyMemory<byte>> ReceiveAsync(int length)
    {
        byte[] buffer = new byte[length == -1 ? client.Available : length];
        await TaskHelper.WaitUntil(() => client.Available >= buffer.Length, cancellationToken: cancellationTokenSource.Token);

        if (cancellationTokenSource.IsCancellationRequested)
        {
            return ReadOnlyMemory<byte>.Empty; // Return empty memory if the client is disconnected
        }

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