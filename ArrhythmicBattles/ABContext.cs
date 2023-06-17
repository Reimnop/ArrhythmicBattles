using ArrhythmicBattles.Core.IO;
using ArrhythmicBattles.Core.Resource;
using ArrhythmicBattles.Settings;
using Config.Net;
using Config.Net.Stores;
using DiscordRPC;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Util.Logging;
using OpenTK.Mathematics;

namespace ArrhythmicBattles;

public class ABContext : IDisposable
{
    public FlexFrameworkMain Engine { get; }
    public ResourceManager ResourceManager { get; }
    public GameTaskManager TaskManager { get; } = new();
    public IRenderBuffer RenderBuffer { get; }
    public DiscordRpcClient DiscordRpcClient { get; }
    public DateTime GameStartedTime { get; }
    public InputSystem InputSystem { get; }
    public ISettings Settings { get; }

    private readonly ILogger logger;

    public ABContext(FlexFrameworkMain engine)
    {
        logger = engine.CreateLogger<ABContext>();
        
        Engine = engine;
        ResourceManager = new ResourceManager(new RelativeFileSystem(Constants.GlobalResourcesPath), Engine);
        RenderBuffer = engine.Renderer.CreateRenderBuffer(Vector2i.One); // 1 * 1 init size
        DiscordRpcClient = InitDiscord();
        GameStartedTime = DateTime.UtcNow;
        InputSystem = new InputSystem(engine.Input);
        
        var configStore = new JsonConfigStore("settings.json", true);
        Settings = new ConfigurationBuilder<ISettings>()
            .UseConfigStore(configStore)
            .Build();
    }

    private DiscordRpcClient InitDiscord()
    {
        var client = new DiscordRpcClient("1002257911063531520");
        
        if (!client.Initialize())
            logger.LogError("Failed to initialize Discord RPC client!");

        return client;
    }

    public void Update(UpdateArgs args)
    {
        TaskManager.Update(args);
        DiscordRpcClient.Invoke();
        InputSystem.Update();
    }

    public void Dispose()
    {
        DiscordRpcClient.Dispose();
    }
}