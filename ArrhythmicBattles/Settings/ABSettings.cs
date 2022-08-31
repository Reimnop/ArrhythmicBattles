using System.Text.Json.Nodes;

namespace ArrhythmicBattles.Settings;

public interface IConfigurable
{
    JsonObject ToJson();
    void FromJson(JsonObject jsonObject);
}

public struct SettingsCategory
{
    public string Name { get; }
    public IConfigurable Configurable { get; }

    public SettingsCategory(string name, IConfigurable configurable)
    {
        Name = name;
        Configurable = configurable;
    }
}

public class ABSettings
{
    private readonly Dictionary<string, IConfigurable> configurables;

    public ABSettings(params SettingsCategory[] categories)
    {
        configurables = categories.ToDictionary(x => x.Name, x => x.Configurable);
    }

    public void FromJson(JsonObject jsonObject)
    {
        foreach (var (name, configurable) in configurables)
        {
            if (!jsonObject.TryGetPropertyValue(name, out JsonNode? node))
            {
                continue;
            }
            if (node is not JsonObject configObject)
            {
                continue;
            }
            
            configurable.FromJson(configObject);
        }
    }

    public JsonObject ToJson()
    {
        JsonObject jsonObject = new JsonObject();
        foreach (var (name, configurable) in configurables)
        {
            jsonObject.Add(name, configurable.ToJson());
        }

        return jsonObject;
    }
}