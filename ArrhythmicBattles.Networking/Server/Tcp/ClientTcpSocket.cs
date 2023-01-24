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

    public override async ValueTask<Memory<byte>> ReceiveAsync()
    {
        if (client.Available == 0)
        {
            return new Memory<byte>();
        }
        
        Memory<byte> buffer = new byte[client.Available];
        await stream.ReadAsync(buffer);
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