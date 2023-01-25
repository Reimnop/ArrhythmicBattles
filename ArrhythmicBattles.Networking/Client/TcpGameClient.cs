using System.Net;
using System.Net.Sockets;

namespace ArrhythmicBattles.Networking.Client;

public class TcpGameClient : GameClient, IDisposable
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    
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

    public override async ValueTask<Memory<byte>> ReceiveAsync()
    {
        if (client.Available == 0)
        {
            return Memory<byte>.Empty;
        }
        
        // I hope the garbage collector doesn't get mad at me for this
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
        client.Dispose();
    }
}