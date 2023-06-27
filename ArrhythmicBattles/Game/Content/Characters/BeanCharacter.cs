using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;

namespace ArrhythmicBattles.Game.Content.Characters;

public class BeanCharacter : Character
{
    public override string Name => "Bean";
    public override AttributeList Attributes => AttributeList.Default;

    public override CharacterInstance CreateInstance(IInputMethod inputMethod, ResourceManager resourceManager, PhysicsWorld physicsWorld)
    {
        return new BeanCharacterInstance(inputMethod, resourceManager, physicsWorld, this);
    }

    public override CharacterPreview CreatePreview(ResourceManager resourceManager)
    {
        return new BeanCharacterPreview(resourceManager);
    }
}
