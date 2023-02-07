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
        
        await Task.WhenAll(acceptTask);
    }

    private async Task AcceptClientsAsync()
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            ClientSocket? client = await socket.AcceptAsync();
            if (client != null)
            {
                Player player = new Player(client);
                players.Add(player);
                
                Console.WriteLine($"Accepted client {client.GetName()}");

                Packet packet = await player.ReceivePacketAsync();
                if (packet is AuthPacket authPacket)
                {
                    Console.WriteLine($"Received auth packet from {authPacket.Username}");
                }
            }
        }
    }
    
    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}