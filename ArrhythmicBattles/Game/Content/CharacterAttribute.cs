namespace ArrhythmicBattles.Game.Content;

public struct CharacterAttribute
{
    public AttributeType Type { get; }
    public int Value { get; }

    public CharacterAttribute(AttributeType type, int value)
    {
        Type = type;
        Value = value;
    }
}