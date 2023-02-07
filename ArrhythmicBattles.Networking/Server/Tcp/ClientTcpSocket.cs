using System.Diagnostics;
using System.Net.Sockets;

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
    
    public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer)
    {
        return stream.WriteAsync(buffer);
    }

    public override async ValueTask<ReadOnlyMemory<byte>> ReceiveAsync()
    {
        if (client.Available == 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }
        
        // I hope the garbage collector doesn't get mad at me for this
        byte[] buffer = new byte[client.Available];
        int bytesRead = await stream.ReadAsync(buffer);
        Debug.Assert(bytesRead == buffer.Length); // This should never happen (hopefully)
        return buffer;
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