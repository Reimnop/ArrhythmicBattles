using ArrhythmicBattles.Networking;
using ArrhythmicBattles.Networking.Packets;
using ArrhythmicBattles.Networking.Server;

namespace ArrhythmicBattles.Server;

public class Player : IDisposable
{
    private readonly ClientSocket client;
    private readonly TypedPacketTunnel tunnel;

    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    internal Player(ClientSocket client)
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
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        client.Close();
    }
}