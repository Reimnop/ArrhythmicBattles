using System.Diagnostics;
using System.Net.Sockets;
using ArrhythmicBattles.Common;

namespace ArrhythmicBattles.Networking.Server.Tcp;

public class ClientTcpSocket : ClientSocket, IDisposable
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;

    public ClientTcpSocket(TcpClient client)
    {
        this.client = client;
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
        Debug.Assert(bytesRead == buffer.Length); // This should never happen
        return buffer;
    }

    public override string GetName()
    {
        return client.Client.RemoteEndPoint!.ToString()!;
    }

    public override void Close()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        client.Close();
    }
}