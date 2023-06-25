using System.Collections;
using ArrhythmicBattles.Game.Content.Props;
using ArrhythmicBattles.Util;

namespace ArrhythmicBattles.Game.Content;

public class PropRegistry : IReadOnlyList<Prop>
{
    public int Count => registry.Count;
    
    private readonly Registry<Prop> registry;

    public PropRegistry()
    {
        registry = new Registry<Prop>(Register);
    }
    
    public Prop this[Identifier identifier] => registry[registry[identifier]];
    public Prop this[int index] => registry.Items[index];

    private static void Register(RegisterDelegate<Prop> register)
    {
        register(new Identifier("grid"), new GridProp());
    }

    public IEnumerator<Prop> GetEnumerator()
    {
        return registry.Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}