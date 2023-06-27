using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using ArrhythmicBattles.Game.Content;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game;

public class PlayerEntity : Entity, IUpdateable, IRenderable, IDisposable
{
    public Vector3 Position
    {
        get => characterInstance.Position;
        set => characterInstance.Position = value;
    }
    
    public Quaternion Rotation
    {
        get => characterInstance.Rotation;
        set => characterInstance.Rotation = value;
    }
    
    private readonly CharacterInstance characterInstance;

    public PlayerEntity(
        Character character, 
        IInputMethod inputMethod, 
        ResourceManager resourceManager, 
        PhysicsWorld physicsWorld)
    {
        // Create character instance
        characterInstance = character.CreateInstance(inputMethod, resourceManager, physicsWorld);
    }

    public void Update(UpdateArgs args)
    {
        characterInstance.Update(args);
    }

    public void Render(RenderArgs args)
    {
        characterInstance.Render(args);
    }
    
    public void Dispose()
    {
        if (characterInstance is IDisposable disposable)
            disposable.Dispose();
    }
}