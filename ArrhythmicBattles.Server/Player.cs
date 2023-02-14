using ArrhythmicBattles.Networking;
using ArrhythmicBattles.Networking.Packets;
using ArrhythmicBattles.Networking.Server;

namespace ArrhythmicBattles.Server;

public class Player : IDisposable
{
    public NetworkHandler NetworkHandler { get; }
    public string? Username { get; private set; }
    public long? Id { get; private set; }
    public bool Authenticated { get; private set; } = false;
    
    private readonly GameServer server;
    private readonly ClientSocket clientSocket;

    private float time = 0.0f;
    private float heartbeatTime = 0.0f;

    public Player(GameServer server, ClientSocket clientSocket)
    {
        this.server = server;
        this.clientSocket = clientSocket;
        NetworkHandler = new NetworkHandler(clientSocket);
    }

    public async Task AuthenticateAsync()
    {
        Console.WriteLine("Waiting for authentication packet...");
        AuthPacket? packet = await NetworkHandler.ReceivePacketAsync<AuthPacket>();

        if (packet is null)
        {
            Console.WriteLine($"Client '{clientSocket.GetName()}' sent invalid packet, disconnecting");
            await server.DisconnectPlayerAsync(this);
            return;
        }

        Username = packet.Username;
        Id = packet.Id;
        Authenticated = true;
        Console.WriteLine($"Client '{clientSocket.GetName()}' authenticated as '{Username}' (ID: {Id})");
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
            
            Exception? exception = NetworkHandler.GetException();
            if (exception != null)
            {
                Console.WriteLine($"Exception in {Username} ({Id}): {exception}");
                await server.DisconnectPlayerAsync(this);
            }
        }
    }

    public void Dispose()
    {
        NetworkHandler.Dispose();
        clientSocket.Close();
    }
}