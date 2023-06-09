using ArrhythmicBattles.Core.IO;
using FlexFramework.Modelling;

namespace ArrhythmicBattles.Core.Resource.Loaders;

public class ModelLoader : IResourceLoader
{
    public bool CanLoad(Type type, IFileSystem fileSystem, string path)
    {
        return type == typeof(Model);
    }

    public object Load(Type type, IFileSystem fileSystem, string path)
    {
        using var stream = fileSystem.Open(path, FileMode.Open);
        return new Model(stream);
    }
}