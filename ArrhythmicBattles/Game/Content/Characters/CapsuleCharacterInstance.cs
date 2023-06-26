using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using BepuPhysics;
using BepuPhysics.Collidables;
using FlexFramework.Core;
using FlexFramework.Modelling;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content.Characters;

public class CapsuleCharacterInstance : CharacterInstance, IDisposable
{
    private const float Mass = 40.0f;

    public override Vector3 Position
    {
        get => controller.Position;
        set => controller.Position = value;
    }
    
    public override Quaternion Rotation
    {
        get => controller.Rotation;
        set => controller.Rotation = value;
    }
    
    private readonly ModelEntity entity;
    private readonly CharacterController controller;

    public CapsuleCharacterInstance(IInputMethod inputMethod, ResourceManager resourceManager, PhysicsWorld physicsWorld, CapsuleCharacter character)
    {
        var model = resourceManager.Get<Model>("Models/Capsule.dae");
        entity = new ModelEntity(model);
        
        // Initialize physics body
        var capsule = new Capsule(0.5f, 1.0f);
        var capsuleIndex = physicsWorld.Simulation.Shapes.Add(capsule);
        var rigidPose = new RigidPose(System.Numerics.Vector3.Zero, System.Numerics.Quaternion.Identity);
        var bodyDescription = BodyDescription.CreateDynamic(
            rigidPose, 
            new BodyInertia { InverseMass = 1.0f / Mass },
            new CollidableDescription(capsuleIndex, 0.1f, float.MaxValue, ContinuousDetection.Passive),
            0.01f);
        
        // Initialize controller
        controller = new CharacterController(character, this, inputMethod, physicsWorld, bodyDescription);
    }

    public override float GetAttributeMultiplier(AttributeType type) => 1.0f;

    public override void Update(UpdateArgs args)
    {
        controller.Update(args);
    }

    public override void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Rotate(Rotation);
        matrixStack.Translate(Position);
        entity.Render(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        controller.Dispose();
    }
}
