namespace ArrhythmicBattles.Common;

public struct Identifier
{
    public string Namespace { get; }
    public string Name { get; }
    
    public Identifier(string @namespace, string name)
    {
        Namespace = @namespace;
        Name = name;
    }
    
    public override string ToString()
    {
        return $"{Namespace}:{Name}";
    }
    
    public static Identifier Parse(string identifier)
    {
        string[] parts = identifier.Split(':');
        return new Identifier(parts[0], parts[1]);
    }
    
    public static implicit operator Identifier(string identifier)
    {
        return Parse(identifier);
    }
    
    public static implicit operator string(Identifier identifier)
    {
        return identifier.ToString();
    }

    public bool Equals(Identifier other)
    {
        return Namespace == other.Namespace && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Namespace, Name);
    }
}