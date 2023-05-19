using FlexFramework.Core.Audio;
using FlexFramework.Modelling;

namespace ArrhythmicBattles.Game.Content;

// Loads and caches content, such as models, textures, and sounds
public class ContentLoader : IDisposable
{
    private static readonly Dictionary<Type, Func<string, object>> ContentFactories = new()
    {
        {typeof(Model), path => new Model(path)},
        {typeof(AudioStream), path =>
        {
            var extension = Path.GetExtension(path);
            switch (extension)
            {
                case "ogg":
                    return new VorbisAudioStream(path);
                default:
                    throw new NotSupportedException($"Audio format '{extension}' is not supported!");
            }
        }}
    };
    
    private readonly Dictionary<string, object> loadedContent = new();
    private readonly string basePath;
    
    public ContentLoader(string basePath)
    {
        this.basePath = basePath;
    }
    
    public T Load<T>(string path) where T : class
    {
        var fullPath = Path.GetFullPath(path, basePath);
        
        if (!ContentFactories.TryGetValue(typeof(T), out var factory))
        {
            throw new NotSupportedException($"Content type '{typeof(T)}' is not supported!");
        }
        
        if (!loadedContent.TryGetValue(fullPath, out var content))
        {
            content = factory(fullPath);
            loadedContent.Add(fullPath, content);
        }
        
        return (T) content;
    }

    public void Dispose()
    {
        foreach (var disposable in loadedContent.Values.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}