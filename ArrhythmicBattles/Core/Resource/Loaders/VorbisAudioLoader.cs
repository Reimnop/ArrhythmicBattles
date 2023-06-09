using ArrhythmicBattles.Core.IO;
using FlexFramework.Core.Audio;

namespace ArrhythmicBattles.Core.Resource.Loaders;

public class VorbisAudioLoader : IResourceLoader
{
    public bool CanLoad(Type type, IFileSystem fileSystem, string path)
    {
        return type == typeof(AudioStream) && path.EndsWith(".ogg");
    }

    public object Load(Type type, IFileSystem fileSystem, string path)
    {
        var stream = fileSystem.Open(path, FileMode.Open);
        return new VorbisAudioStream(stream);
    }
}