using ArrhythmicBattles.Networking;
using ArrhythmicBattles.Networking.Packets;
using ArrhythmicBattles.Networking.Server;

namespace ArrhythmicBattles.Server;

public class PlayerNetworkHandler : IDisposable
{
    private readonly ClientSocket client;
    private readonly TypedPacketTunnel tunnel;
    private readonly PacketCollector collector;
    
    public PlayerNetworkHandler(ClientSocket client)
    {
        this.client = client;
        tunnel = new TypedPacketTunnel(client);
        collector = new PacketCollector(tunnel);
    }
    
    public async Task SendPacketAsync(Packet packet)
    {
        await tunnel.SendAsync(packet);
    }

    public Task<T?> ReceivePacketAsync<T>() where T : Packet
    {
        return collector.ReceivePacketAsync<T>();
    }
    
    public Task<T?> GetPacketAsync<T>() where T : Packet
    {
        return collector.GetPacketAsync<T>();
    }

    public void Dispose()
    {
        client.Close();
        collector.Dispose();
    }
}