using ArrhythmicBattles.Core.IO;
using Newtonsoft.Json.Linq;

namespace ArrhythmicBattles.Core.Resource.Loaders;

public class ResourceDictionaryLoader : IResourceLoader
{
    public bool CanLoad(Type type, IFileSystem fileSystem, string path)
    {
        return type == typeof(ResourceDictionary) && path.EndsWith(".json");
    }

    public object Load(Type type, IFileSystem fileSystem, string path)
    {
        using var stream = fileSystem.Open(path, FileMode.Open);
        var reader = new StreamReader(stream);
        var json = JObject.Parse(reader.ReadToEnd());
        var entries = new Dictionary<string, string>();
        foreach (var (name, value) in json)
        {
            entries[name] = value.Value<string>();
        }
        return new ResourceDictionary(entries.Select(entry => (entry.Key, entry.Value)));
    }
}