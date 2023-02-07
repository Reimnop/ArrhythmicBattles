using System.Globalization;
using System.Net;
using System.Text;
using ArrhythmicBattles.Networking.Client;
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
        Console.WriteLine("Client connected to server");
        
        ReadOnlyMemory<byte> buffer = await client.ReceiveAsync();
        Console.WriteLine($"Client received data from server ({buffer.Length} bytes)");
        
        int length = BitConverter.ToInt32(buffer.Span);
        string message = Encoding.UTF8.GetString(buffer.Span.Slice(4, length));
        
        Console.WriteLine($"Client received '{message}'");
    }
}