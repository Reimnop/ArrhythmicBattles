namespace ArrhythmicBattles.Util;

public struct RegistryLocation<T> where T : class
{
    public int Index { get; }
    private readonly Registry<T> owner;
    
    public RegistryLocation(Registry<T> owner, int index)
    {
        this.owner = owner;
        Index = index;
    }
    
    public bool IsOwnedBy(Registry<T> registry)
    {
        return owner == registry;
    }
    
    public static explicit operator T(RegistryLocation<T> location)
    {
        return location.owner[location];
    }
}