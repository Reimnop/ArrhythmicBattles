using ArrhythmicBattles.Networking.Packets;

namespace ArrhythmicBattles.Server;

public class Player : IDisposable
{
    public PlayerNetworkHandler NetworkHandler { get; }
    public string Username { get; }
    public long Id { get; }
    
    private readonly GameServer server;

    private float time = 0.0f;
    private float heartbeatTime = 0.0f;

    public Player(GameServer server, PlayerNetworkHandler networkHandler, string username, long id)
    {
        this.server = server;
        NetworkHandler = networkHandler;
        Username = username;
        Id = id;
    }

    public async Task TickAsync(float deltaTime)
    {
        time += deltaTime;
        heartbeatTime += deltaTime;

        if (heartbeatTime >= 5.0f)
        {
            heartbeatTime = 0.0f;

            try
            {
                Console.WriteLine($"Sending heartbeat to {Username} ({Id})");
                await NetworkHandler.SendPacketAsync(new HeartbeatPacket());
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to send heartbeat to {Username} ({Id}), disconnecting client");
                await server.DisconnectPlayerAsync(this);
            }
        }
    }

    public void Dispose()
    {
        NetworkHandler.Dispose();
    }
}