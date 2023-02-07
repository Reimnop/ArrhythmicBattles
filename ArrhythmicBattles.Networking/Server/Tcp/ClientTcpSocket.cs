using System.Diagnostics;
using System.Net.Sockets;
using ArrhythmicBattles.Common;

namespace ArrhythmicBattles.Networking.Server.Tcp;

public class ClientTcpSocket : ClientSocket, IDisposable
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    
    public ClientTcpSocket(TcpClient client)
    {
        this.client = client;
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
        int bytesRead = await stream.ReadAsync(buffer);
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
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        client.Close();
    }
}