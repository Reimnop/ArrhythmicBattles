using ArrhythmicBattles.Networking.Client;

namespace ArrhythmicBattles.Server;

public class Player : IDisposable
{
    private readonly GameClient client;
    
    internal Player(GameClient client)
    {
        this.client = client;
    }

    public void Dispose()
    {
        client.Close();
    }
}