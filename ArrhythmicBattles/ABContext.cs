using ArrhythmicBattles.Util;
using DiscordRPC;
using FlexFramework;

namespace ArrhythmicBattles;

public class ABContext : IDisposable
{
    public DiscordRpcClient DiscordRpcClient { get; }
    public DateTime GameStartedTime { get; }
    public InputSystem InputSystem { get; }

    public ABContext(FlexFrameworkMain engine)
    {
        DiscordRpcClient = InitDiscord();
        GameStartedTime = DateTime.UtcNow;
        InputSystem = new InputSystem(engine.Input);
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