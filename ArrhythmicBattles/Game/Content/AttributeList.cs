namespace ArrhythmicBattles.Game.Content;

public class AttributeList
{
    public static AttributeList Default { get; } = new(null,
        new CharacterAttribute(AttributeType.Attack, 10),
        new CharacterAttribute(AttributeType.Defense, 10),
        new CharacterAttribute(AttributeType.BaseSpeed, 5),
        new CharacterAttribute(AttributeType.AirSpeed, 6),
        new CharacterAttribute(AttributeType.GroundSpeed, 5),
        new CharacterAttribute(AttributeType.Acceleration, 2),
        new CharacterAttribute(AttributeType.JumpDistance, 4),
        new CharacterAttribute(AttributeType.JumpCount, 2)
    );

    private readonly Dictionary<AttributeType, CharacterAttribute> attributes;
    private readonly AttributeList? parent;

    public AttributeList(AttributeList? parent, params CharacterAttribute[] attributes)
    {
        this.parent = parent;
        this.attributes = new Dictionary<AttributeType, CharacterAttribute>();
        foreach (var attribute in attributes)
        {
            this.attributes.Add(attribute.Type, attribute);
        }
    }

    public AttributeList(params CharacterAttribute[] attributes) : this(Default, attributes)
    {
    }

    public void SetAttribute(CharacterAttribute attribute)
    {
        attributes[attribute.Type] = attribute;
    }

    public void SetAttribute(AttributeType type, int value)
    {
        attributes[type] = new CharacterAttribute(type, value);
    }

    public CharacterAttribute GetAttribute(AttributeType type) 
    {
        if (attributes.TryGetValue(type, out var attribute))
        {
            return attribute;
        }
        
        if (parent != null)
        {
            return parent.GetAttribute(type);
        }

        throw new KeyNotFoundException($"Attribute '{type}' not found!");
    }
}
