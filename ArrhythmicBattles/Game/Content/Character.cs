using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using ArrhythmicBattles.UserInterface;
using BepuPhysics.Collidables;

namespace ArrhythmicBattles.Game.Content;

public abstract class Character : INamed
{
    public abstract string Name { get; }
    public abstract AttributeList Attributes { get; }

    public abstract CharacterInstance CreateInstance(IInputMethod inputMethod, ResourceManager resourceManager, PhysicsWorld physicsWorld);
    public abstract CharacterPreview CreatePreview(ResourceManager resourceManager);
    
    public float GetAttributeValue(CharacterInstance instance, AttributeType type)
    {
        return GetAttributeValue(this, instance, type);
    }
    
    public static float GetAttributeValue(Character character, CharacterInstance instance, AttributeType type)
    {
        return character.Attributes.GetAttribute(type).Value * instance.GetAttributeMultiplier(type);
    }
}
