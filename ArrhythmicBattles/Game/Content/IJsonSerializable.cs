using Newtonsoft.Json.Linq;

namespace ArrhythmicBattles.Game.Content;

public interface IJsonSerializable<out T>
{
    JObject ToJson();
    static abstract T FromJson(JObject json);
}