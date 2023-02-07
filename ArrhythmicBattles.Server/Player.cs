namespace ArrhythmicBattles.Server;

public class Player : IDisposable
{
    public PlayerNetworkHandler NetworkHandler { get; }
    public string Username { get; }
    public long Id { get; }

    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    internal Player(PlayerNetworkHandler networkHandler, string username, long id)
    {
        NetworkHandler = networkHandler;
        Username = username;
        Id = id;
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        NetworkHandler.Dispose();
    }
}