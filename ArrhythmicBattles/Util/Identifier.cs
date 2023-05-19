namespace ArrhythmicBattles.Util;

public struct Identifier
{
    public string Namespace { get; set; }
    public string Name { get; set; }
    
    public Identifier(string @namespace, string name)
    {
        Namespace = @namespace;
        Name = name;
    }

    internal Identifier(string name)
    {
        Namespace = Constants.DefaultNamespace;
        Name = name;
    }
    
    public override string ToString()
    {
        return $"{Namespace}:{Name}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Namespace, Name);
    }

    public static implicit operator Identifier(string s)
    {
        var split = s.Split(':', 2);
        if (split.Length != 2)
        {
            throw new InvalidCastException($"Cannot cast string \"{s}\" to {nameof(Identifier)}");
        }
        
        return new Identifier(split[0], split[1]);
    }
    
    public static implicit operator string(Identifier id)
    {
        return id.ToString();
    }
}