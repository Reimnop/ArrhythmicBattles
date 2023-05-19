using Newtonsoft.Json.Linq;

namespace ArrhythmicBattles.Game.Content;

public class MapMeta : IJsonSerializable<MapMeta>
{
    public string Name { get; }
    public string Music { get; }
    public IReadOnlyList<PropInfo> Props { get; }
    
    public MapMeta(string name, string music, IEnumerable<PropInfo> props)
    {
        Name = name;
        Music = music;
        Props = props.ToList();
    }

    public JObject ToJson()
    {
        return new JObject
        {
            ["name"] = Name,
            ["music"] = Music,
            ["props"] = new JArray(Props.Select(prop => prop.ToJson()))
        };
    }

    public static MapMeta FromJson(JObject json)
    {
        return new MapMeta(
            json["name"].Value<string>(),
            json["music"].Value<string>(),
            json["props"].Select(token => PropInfo.FromJson((JObject) token))
        );
    }
}