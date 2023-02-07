using System.Globalization;
using System.Net;
using System.Text;
using ArrhythmicBattles.Networking;
using ArrhythmicBattles.Networking.Client;
using ArrhythmicBattles.Networking.Packets;
using ArrhythmicBattles.Networking.Server;
using ArrhythmicBattles.Networking.Server.Local;
using ArrhythmicBattles.Networking.Server.Tcp;
using ArrhythmicBattles.Server;

namespace NetworkTest;

class Program
{
    private static ServerTcpSocket serverSocket;
    
    public static void Main(string[] args)
    {
        serverSocket = new ServerTcpSocket(19738);
        
        GameServer server = new GameServer(serverSocket);
        Task.Run(() => server.Start());
        Task.Run(Client);
        
        Thread.Sleep(Timeout.Infinite);
    }

    private static async Task Client()
    {
        Console.WriteLine("Starting client");

        GameClient client = new TcpGameClient(IPAddress.Loopback, 19738);
        TypedPacketTunnel tunnel = new TypedPacketTunnel(client);
        Console.WriteLine("Client connected to server");
        
        await tunnel.SendAsync(new AuthPacket("TestUser"));
        Console.WriteLine("Sent auth packet");
    }
}