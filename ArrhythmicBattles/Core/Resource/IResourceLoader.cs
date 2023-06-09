using ArrhythmicBattles.Core.IO;

namespace ArrhythmicBattles.Core.Resource;

public interface IResourceLoader
{
    bool CanLoad(Type type, IFileSystem fileSystem, string path);
    object Load(Type type, IFileSystem fileSystem, string path);
}