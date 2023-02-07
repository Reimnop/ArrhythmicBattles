using System.Net;
using ArrhythmicBattles.Networking;
using ArrhythmicBattles.Networking.Client;
using ArrhythmicBattles.Networking.Packets;
using ArrhythmicBattles.Networking.Server.Tcp;
using ArrhythmicBattles.Server;

namespace NetworkTest;

class Program
{
    private static ServerTcpSocket serverSocket;
    
    public static void Main(string[] args)
    {
        serverSocket = new ServerTcpSocket(42069); // Funni number 
        
        GameServer server = new GameServer(serverSocket);
        Task.Run(() => server.Start());
        Task.Run(Client);
        
        Thread.Sleep(Timeout.Infinite);
    }

    private static async Task Client()
    {
        GameClient client = new TcpGameClient(IPAddress.Loopback, 42069);
        TypedPacketTunnel tunnel = new TypedPacketTunnel(client);
        await tunnel.SendAsync(new AuthPacket("TestUser"));
    }
}