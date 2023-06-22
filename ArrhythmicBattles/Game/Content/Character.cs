using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using BepuPhysics.Collidables;

namespace ArrhythmicBattles.Game.Content;

public abstract class Character
{
    public abstract string Name { get; }
    public abstract AttributeList Attributes { get; }

    public abstract CharacterInstance CreateInstance(IInputMethod inputMethod, ResourceManager resourceManager, PhysicsWorld physicsWorld);
    public abstract CharacterPreview CreatePreview(ResourceManager resourceManager);
}
