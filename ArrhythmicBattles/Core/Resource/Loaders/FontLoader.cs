using ArrhythmicBattles.Core.IO;
using FlexFramework.Text;

namespace ArrhythmicBattles.Core.Resource.Loaders;

public class FontLoader : IResourceLoader
{
    public bool CanLoad(Type type, IFileSystem fileSystem, string path)
    {
        return type == typeof(Font);
    }

    public object Load(Type type, IFileSystem fileSystem, string path)
    {
        using var stream = fileSystem.Open(path, FileMode.Open);
        return FontDeserializer.Deserialize(stream);
    }
}