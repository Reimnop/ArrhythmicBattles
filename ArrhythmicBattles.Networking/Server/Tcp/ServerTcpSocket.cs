using System.Net;
using System.Net.Sockets;

namespace ArrhythmicBattles.Networking.Server.Tcp;

public class ServerTcpSocket : ServerSocket, IDisposable
{
    private readonly TcpListener listener;
    
    public ServerTcpSocket(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
    }

    public override async Task<ClientSocket?> AcceptAsync()
    {
        try
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            return new ClientTcpSocket(client);
        }
        catch (SocketException)
        {
            return null;
        }
    }

    public override Task DisconnectClientAsync(ClientSocket clientSocket)
    {
        clientSocket.Close();
        return Task.CompletedTask;
    }

    public override void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        listener.Stop();
    }
}