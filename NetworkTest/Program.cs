using ArrhythmicBattles.Networking.Client;
using ArrhythmicBattles.Networking.Server;
using ArrhythmicBattles.Networking.Server.Local;

namespace NetworkTest;

class Program
{
    public static void Main(string[] args)
    {
        ServerLocalSocket server = new ServerLocalSocket();
        Task.Run(() => Server(server));
        Client(server).GetAwaiter().GetResult();
        
        Thread.Sleep(1000);
    }

    public static async Task Client(ServerLocalSocket server)
    {
        GameClient gameClient = await server.ConnectLocalClientAsync();
        Console.WriteLine("[CLIENT] Connected to server");
        
        Console.WriteLine("[CLIENT] Sending data");
        byte[] data = new byte[2048];
        await gameClient.SendAsync(data);

        Console.WriteLine("[CLIENT] Receiving data");
        ReadOnlyMemory<byte> buffer = await gameClient.ReceiveAsync();
        Console.WriteLine($"[CLIENT] Received {buffer.Length} bytes");
    }

    public static async Task Server(ServerSocket serverSocket)
    {
        ClientSocket? socket = await serverSocket.AcceptAsync();

        if (socket != null)
        {
            Console.WriteLine("[SERVER] Client connected");
            
            ReadOnlyMemory<byte> buffer = await socket.ReceiveAsync();
            Console.WriteLine($"[SERVER] Received {buffer.Length} bytes");
            
            Console.WriteLine("[SERVER] Sending data");
            byte[] data = new byte[1024];
            await socket.SendAsync(data);
        }
    }
}