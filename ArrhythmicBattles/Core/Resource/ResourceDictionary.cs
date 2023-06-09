namespace ArrhythmicBattles.Core.Resource;

public class ResourceDictionary
{
    private readonly Dictionary<string, string> entries;
    
    public ResourceDictionary(IEnumerable<(string, string)> entries)
    {
        this.entries = entries.ToDictionary(entry => entry.Item1, entry => entry.Item2);
    }

    /// <summary>
    /// Gets the raw value of the given key.
    /// </summary>
    /// <returns>The value.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key does not exist.</exception>
    public string GetRaw(string key)
    {
        // Check for wildcard entry
        if (entries.TryGetValue("*", out var value))
        {
            return value;
        }

        // Check for specific entry
        if (!entries.TryGetValue(key, out value))
        {
            throw new KeyNotFoundException($"No entry with key \"{key}\" exists in this resource dictionary!");
        }

        return value;
    }

    /// <summary>
    /// Gets the value of the given key and loads the resource at the path specified by the value.
    /// </summary>
    /// <returns>The loaded resource.</returns>
    public object LoadResource(Type type, string key, ResourceManager resourceManager)
    {
        var value = GetRaw(key);
        return resourceManager.Load(type, value);
    }
    
    /// <summary>
    /// Gets the value of the given key and loads the resource at the path specified by the value.
    /// </summary>
    /// <returns>The loaded resource.</returns>
    public T LoadResource<T>(string key, ResourceManager resourceManager)
    {
        return (T) LoadResource(typeof(T), key, resourceManager);
    }
}