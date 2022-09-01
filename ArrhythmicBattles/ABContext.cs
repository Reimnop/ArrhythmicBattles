using System.Text.Json;
using System.Text.Json.Nodes;
using ArrhythmicBattles.Settings;
using ArrhythmicBattles.Util;
using DiscordRPC;
using FlexFramework;

namespace ArrhythmicBattles;

public class ABContext : IDisposable
{
    public DiscordRpcClient DiscordRpcClient { get; }
    public DateTime GameStartedTime { get; }
    public InputSystem InputSystem { get; }
    public ABSound Sound { get; }
    public ABSettings Settings { get; }

    public ABContext(FlexFrameworkMain engine)
    {
        DiscordRpcClient = InitDiscord();
        GameStartedTime = DateTime.UtcNow;
        InputSystem = new InputSystem(engine.Input);
        Sound = new ABSound();

        Settings = new ABSettings(new SettingsCategory("sound", Sound));
        LoadSettings();
    }

    public void SaveSettings()
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        
        File.WriteAllText("settings.json", Settings.ToJson().ToJsonString(options));
    }
    
    public void LoadSettings()
    {
        if (File.Exists("settings.json"))
        {
            JsonNode? settingsNode = JsonNode.Parse(File.ReadAllText("settings.json"));

            if (settingsNode is JsonObject jsonObject)
            {
                Settings.FromJson(jsonObject);
            }
        }
    }

    private DiscordRpcClient InitDiscord()
    {
        DiscordRpcClient client = new DiscordRpcClient("1002257911063531520");
        client.Initialize();

        return client;
    }

    public void Update()
    {
        Sound.Update();
        DiscordRpcClient.Invoke();
    }

    public void Dispose()
    {
        DiscordRpcClient.Dispose();
        Sound.Dispose();
    }
}