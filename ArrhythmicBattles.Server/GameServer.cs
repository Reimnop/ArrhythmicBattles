using System.Diagnostics;
using System.Text;
using ArrhythmicBattles.Common;
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
        Task tickTask = TickLoopAsync();
        
        await Task.WhenAll(acceptTask, tickTask);
    }
    
    private async Task TickLoopAsync()
    {
        const int tickRate = 20;
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            await TickAsync();

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

    private async Task TickAsync()
    {
        
    }

    private async Task AcceptClientsAsync()
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            ClientSocket? client = await socket.AcceptAsync();
            if (client != null)
            {
                Console.WriteLine($"Client '{client.GetName()}' is trying to connect");
                _ = HandleClientAsync(client);
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
            long id = authPacket.Id;
            Console.WriteLine($"Client '{clientSocket.GetName()}' authenticated as '{username}' ({id})");

            // Get player list
            List<PlayerProfile> playerList = players.Select(player => new PlayerProfile(player.Username, player.Id)).ToList();
            playerList.Add(new PlayerProfile(username, id));

            PlayerListPacket playerListPacket = new PlayerListPacket(playerList);
            
            // Send the player list
            await networkHandler.SendPacketAsync(playerListPacket);
            
            // Send other clients player list and join packet
            PlayerJoinPacket playerJoinPacket = new PlayerJoinPacket(id);
            foreach (Player player1 in players)
            {
                await player1.NetworkHandler.SendPacketAsync(playerListPacket);
                await player1.NetworkHandler.SendPacketAsync(playerJoinPacket);
            }

            Player player = new Player(networkHandler, username, id);
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