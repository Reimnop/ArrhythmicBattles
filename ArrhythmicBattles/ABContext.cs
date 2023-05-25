using ArrhythmicBattles.Settings;
using ArrhythmicBattles.Core;
using Config.Net;
using Config.Net.Stores;
using DiscordRPC;
using FlexFramework;
using FlexFramework.Core.Rendering;
using FlexFramework.Text;
using OpenTK.Mathematics;

namespace ArrhythmicBattles;

public class ABContext : IDisposable
{
    public FlexFrameworkMain Engine { get; }
    public IRenderBuffer RenderBuffer { get; }
    public DiscordRpcClient DiscordRpcClient { get; }
    public DateTime GameStartedTime { get; }
    public InputSystem InputSystem { get; }
    public Font Font { get; }
    public ISettings Settings { get; }

    public ABContext(FlexFrameworkMain engine)
    {
        Engine = engine;
        RenderBuffer = engine.Renderer.CreateRenderBuffer(Vector2i.One); // 1 * 1 init size
        DiscordRpcClient = InitDiscord();
        GameStartedTime = DateTime.UtcNow;
        InputSystem = new InputSystem(engine.Input);
        
        using (var stream = File.OpenRead("Assets/Fonts/Inconsolata-Regular.flexfont"))
        {
            Font = FontDeserializer.Deserialize(stream);
        }

        var configStore = new JsonConfigStore("settings.json", true);
        Settings = new ConfigurationBuilder<ISettings>()
            .UseConfigStore(configStore)
            .Build();
    }

    private DiscordRpcClient InitDiscord()
    {
        var client = new DiscordRpcClient("1002257911063531520");
        client.Initialize();

        return client;
    }

    public void Update()
    {
        DiscordRpcClient.Invoke();
        InputSystem.Update();
    }

    public void Dispose()
    {
        DiscordRpcClient.Dispose();
    }
}