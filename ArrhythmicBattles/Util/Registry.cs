namespace ArrhythmicBattles.Util;

public delegate RegistryLocation<T> RegisterDelegate<T>(Identifier identifier, T item) where T : class;
public delegate void RegisterCallback<T>(RegisterDelegate<T> registerDelegate) where T : class;

public class Registry<T> : IDisposable where T : class
{
    public int Count => items.Count;
    public IEnumerable<Identifier> Identifiers => locations.Keys;
    public IEnumerable<T> Items => items;
    
    private readonly Dictionary<Identifier, RegistryLocation<T>> locations = new();
    private readonly List<T> items = new();
    
    public Registry(RegisterCallback<T> registerCallback)
    {
        var registerDelegate = new RegisterDelegate<T>(Register);
        registerCallback(registerDelegate);
    }

    private RegistryLocation<T> Register(Identifier identifier, T item) 
    {
        var location = new RegistryLocation<T>(this, items.Count);
        locations.Add(identifier, location);
        items.Add(item);
        return location;
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