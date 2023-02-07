using System.Diagnostics;
using System.Text;
using ArrhythmicBattles.Networking.Packets;
using ArrhythmicBattles.Networking.Server;

namespace ArrhythmicBattles.Server;

public class GameServer : IDisposable
{
    private readonly List<Player> players = new List<Player>();
    private readonly ServerSocket socket;
    
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public GameServer(ServerSocket socket)
    {
        this.socket = socket;
    }

    public async Task Start()
    {
        Task acceptTask = AcceptClientsAsync();
        Task tickTask = TickAsync();
        
        await Task.WhenAll(acceptTask, tickTask);
    }
    
    private async Task TickAsync()
    {
        const int tickRate = 20;
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            foreach (Player player in players)
            {
                TextPacket textPacket = new TextPacket("Hello World!");
                await player.NetworkHandler.SendPacketAsync(textPacket);
            }

            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            long targetMilliseconds = 1000 / tickRate;
            long sleepMilliseconds = targetMilliseconds - elapsedMilliseconds;
            
            if (sleepMilliseconds > 0)
            {
                await Task.Delay((int) sleepMilliseconds);
            }
            
            stopwatch.Restart();
        }
        
        stopwatch.Stop();
    }

    private async Task AcceptClientsAsync()
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            ClientSocket? client = await socket.AcceptAsync();
            if (client != null)
            {
                Console.WriteLine($"Client '{client.GetName()}' is trying to connect");
                Task.Run(() => HandleClientAsync(client));
            }
        }
    }

    private async Task HandleClientAsync(ClientSocket clientSocket)
    {
        PlayerNetworkHandler networkHandler = new PlayerNetworkHandler(clientSocket);
        Packet packet = await networkHandler.ReceivePacketAsync();

        if (packet is AuthPacket authPacket)
        {
            string username = authPacket.Username;
            Console.WriteLine($"Client '{clientSocket.GetName()}' authenticated as '{username}'");
            
            Player player = new Player(networkHandler, username);
            players.Add(player);
        }
    }
    
    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        socket.Close();
    }
}