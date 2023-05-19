namespace ArrhythmicBattles.Util;

public class Registry<T> : IDisposable where T : class
{
    public int Count => items.Count;
    public IEnumerable<Identifier> Identifiers => locations.Keys;
    public IEnumerable<T> Items => items;
    
    private readonly Dictionary<Identifier, RegistryLocation<T>> locations = new();
    private readonly List<T> items = new();
    
    public Registry(IEnumerable<(Identifier, T)> items)
    {
        foreach (var (id, item) in items)
        {
            locations.Add(id, new RegistryLocation<T>(this, this.items.Count));
            this.items.Add(item);
        }
    }

    public RegistryLocation<T> this[Identifier id]
    {
        get
        {
            if (!locations.TryGetValue(id, out var location))
            {
                throw new KeyNotFoundException($"No item with identifier \"{id}\" exists in this registry!");
            }
            
            return location;
        }
    }
    
    public T this[RegistryLocation<T> location]
    {
        get
        {
            if (!location.IsOwnedBy(this))
            {
                throw new ArgumentException("The given location is not owned by this registry!", nameof(location));
            }
            
            return items[location.Index];
        }
    }

    public void Dispose()
    {
        foreach (var disposable in items.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}