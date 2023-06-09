using ArrhythmicBattles.Core.IO;
using ArrhythmicBattles.Game.Content;
using Newtonsoft.Json.Linq;

namespace ArrhythmicBattles.Core.Resource.Loaders;

public class MapMetaLoader : IResourceLoader
{
    public bool CanLoad(Type type, IFileSystem fileSystem, string path)
    {
        return type == typeof(MapMeta) && path.EndsWith(".json");
    }

    public object Load(Type type, IFileSystem fileSystem, string path)
    {
        using var stream = fileSystem.Open(path, FileMode.Open);
        var reader = new StreamReader(stream);
        var json = JObject.Parse(reader.ReadToEnd());
        return MapMeta.FromJson(json);
    }
}