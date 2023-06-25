using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;

namespace ArrhythmicBattles.Game.Content.Characters;

public class CapsuleCharacter : Character
{
    public override string Name => "Capsule";
    public override AttributeList Attributes { get; } = AttributeList.Default;

    public override CharacterInstance CreateInstance(IInputMethod inputMethod, ResourceManager resourceManager, PhysicsWorld physicsWorld)
    {
        return new CapsuleCharacterInstance(inputMethod, resourceManager, physicsWorld);
    }

    public override CharacterPreview CreatePreview(ResourceManager resourceManager)
    {
        return new CapsuleCharacterPreview(resourceManager);
    }
}
