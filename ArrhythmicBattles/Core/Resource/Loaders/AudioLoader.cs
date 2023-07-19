using ArrhythmicBattles.Core.IO;
using FlexFramework.Core.Audio;

namespace ArrhythmicBattles.Core.Resource.Loaders;

public class AudioLoader : IResourceLoader
{
    public bool CanLoad(Type type, IFileSystem fileSystem, string path)
    {
        return type == typeof(AudioData);
    }

    public object Load(Type type, IFileSystem fileSystem, string path)
    {
        var audioPath = fileSystem.GetFullPath(path);
        return new AudioData(audioPath);
    }
}