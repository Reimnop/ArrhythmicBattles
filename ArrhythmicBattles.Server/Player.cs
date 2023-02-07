namespace ArrhythmicBattles.Server;

public class Player : IDisposable
{
    public PlayerNetworkHandler NetworkHandler { get; }
    public string Username { get; }

    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    internal Player(PlayerNetworkHandler networkHandler, string username)
    {
        NetworkHandler = networkHandler;
        Username = username;
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        NetworkHandler.Dispose();
    }
}