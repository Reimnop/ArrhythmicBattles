using DiscordRPC;

namespace ArrhythmicBattles;

public class ABContext : IDisposable
{
    public DiscordRpcClient DiscordRpcClient { get; }

    public ABContext()
    {
        DiscordRpcClient = InitDiscord();
    }

    private DiscordRpcClient InitDiscord()
    {
        DiscordRpcClient client = new DiscordRpcClient("1002257911063531520");
        client.Initialize();

        return client;
    }

    public void Update()
    {
        DiscordRpcClient.Invoke();
    }

    public void Dispose()
    {
        DiscordRpcClient.Dispose();
    }
}