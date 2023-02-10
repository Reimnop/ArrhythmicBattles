using ArrhythmicBattles.Networking.Packets;

namespace ArrhythmicBattles.Server;

public class Player : IDisposable
{
    public PlayerNetworkHandler NetworkHandler { get; }
    public string Username { get; }
    public long Id { get; }

    private float time = 0.0f;
    private float heartbeatTime = 0.0f;

    public Player(PlayerNetworkHandler networkHandler, string username, long id)
    {
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
            
            Console.WriteLine($"Sending heartbeat to {Username} ({Id})");
            await NetworkHandler.SendPacketAsync(new HeartbeatPacket());
        }
    }

    public void Dispose()
    {
        NetworkHandler.Dispose();
    }
}