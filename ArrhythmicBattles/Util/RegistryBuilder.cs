namespace ArrhythmicBattles.Util;

public class RegistryBuilder<T> where T : class
{
    private readonly List<Identifier> identifiers = new();
    private readonly List<Func<T>> itemFactories = new();
    
    public Registry<T> Build()
    {
        return new Registry<T>(identifiers.Zip(itemFactories.Select(x => x())));
    }
    
    public RegistryBuilder<T> Add(Identifier identifier, Func<T> itemFactory)
    {
        identifiers.Add(identifier);
        itemFactories.Add(itemFactory);
        return this;
    }
}