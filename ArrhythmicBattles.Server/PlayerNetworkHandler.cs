using ArrhythmicBattles.Networking;
using ArrhythmicBattles.Networking.Packets;
using ArrhythmicBattles.Networking.Server;

namespace ArrhythmicBattles.Server;

public class PlayerNetworkHandler : IDisposable
{
    private readonly TypedPacketTunnel tunnel;
    private readonly ClientSocket client;
    
    public PlayerNetworkHandler(ClientSocket client)
    {
        this.client = client;
        tunnel = new TypedPacketTunnel(client);
    }
    
    public async Task SendPacketAsync(Packet packet)
    {
        await tunnel.SendAsync(packet);
    }
    
    public async Task<Packet> ReceivePacketAsync()
    {
        return await tunnel.ReceiveAsync();
    }

    public void Dispose()
    {
        client.Close();
    }
}