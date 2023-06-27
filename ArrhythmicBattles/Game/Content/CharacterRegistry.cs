using System.Collections;
using ArrhythmicBattles.Game.Content.Characters;
using ArrhythmicBattles.Util;

namespace ArrhythmicBattles.Game.Content;

public class CharacterRegistry : IReadOnlyList<Character>
{
    public int Count => registry.Count;
    
    private readonly Registry<Character> registry;

    public CharacterRegistry()
    {
        registry = new Registry<Character>(Register);
    }
    
    public Character this[Identifier identifier] => registry[registry[identifier]];
    public Character this[int index] => registry.Items[index];

    private static void Register(RegisterDelegate<Character> register)
    {
        register(new Identifier("capsule"), new BeanCharacter());
    }

    public IEnumerator<Character> GetEnumerator()
    {
        return registry.Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}