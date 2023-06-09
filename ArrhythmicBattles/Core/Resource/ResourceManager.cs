using ArrhythmicBattles.Core.IO;
using ArrhythmicBattles.Core.Resource.Loaders;

namespace ArrhythmicBattles.Core.Resource;

/// <summary>
/// Manages game resources (textures, sounds, etc.)
/// </summary>
public class ResourceManager : IDisposable
{
    private static readonly IResourceLoader[] ResourceLoaders = 
    {
        new ResourceDictionaryLoader(),
        new TextureSamplerLoader(),
        new VorbisAudioLoader(),
        new ModelLoader(),
        new MapMetaLoader(),
        new WindowIconLoader(),
        new FontLoader()
    };

    private readonly Dictionary<string, object> loadedResources = new();
    private readonly IFileSystem fileSystem;
    
    public ResourceManager(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    /// <summary>
    /// <para>Loads a resource from the file system.</para>
    /// <para>DO NOT dispose the returned object!</para>
    /// </summary>
    /// <returns>The resource.</returns>
    public T Load<T>(string path)
    {
        return (T) Load(typeof(T), path);
    }

    /// <summary>
    /// <para>Loads a resource from the file system.</para>
    /// <para>DO NOT dispose the returned object!</para>
    /// </summary>
    /// <returns>The resource.</returns>
    public object Load(Type type, string path)
    {
        if (loadedResources.TryGetValue(path, out var resource))
        {
            return resource;
        }
        
        var resourceLoader = GetLoader(type, path);
        resource = resourceLoader.Load(type, fileSystem, path);
        loadedResources.Add(path, resource);
        return resource;
    }

    private IResourceLoader GetLoader(Type type, string path)
    {
        foreach (var loader in ResourceLoaders)
        {
            if (loader.CanLoad(type, fileSystem, path))
            {
                return loader;
            }
        }
        
        throw new ArgumentException($"No resource loader found for type '{type}' and path '{path}'!");
    }

    public void Dispose()
    {
        foreach (var disposable in loadedResources.Values.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}