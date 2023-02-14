using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using ArrhythmicBattles.Common;

namespace ArrhythmicBattles.Networking.Client;

public class TcpGameClient : GameClient, IDisposable
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;

    public TcpGameClient(IPAddress address, int port)
    {
        client = new TcpClient();
        client.NoDelay = true;
        client.Connect(address, port);
        stream = client.GetStream();
    }
    
    public override async Task SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await stream.WriteAsync(buffer, cancellationToken);
    }

    public override async Task<ReadOnlyMemory<byte>> ReceiveAsync(int length, CancellationToken cancellationToken = default)
    {
        byte[] buffer = new byte[length == -1 ? client.Available : length];
        await TaskHelper.WaitUntil(() => client.Available >= buffer.Length, cancellationToken: cancellationToken);
        int bytesRead = await stream.ReadAsync(buffer, cancellationToken);
        Debug.Assert(bytesRead == buffer.Length); // Better safe than sorry
        return buffer;
    }

    public override void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        client.Dispose();
    }
}