using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using ArrhythmicBattles.Util;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using FlexFramework.Core;
using FlexFramework.Modelling;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content.Characters;

public class CapsuleCharacterInstance : CharacterInstance, IDisposable
{
    private const float mass = 40.0f;

    public override Vector3 Position
    {
        get => physicsEntity.Position;
        set => physicsEntity.Position = value;
    }
    
    public override Quaternion Rotation
    {
        get => physicsEntity.Rotation;
        set => physicsEntity.Rotation = value;
    }

    private readonly IInputMethod inputMethod;
    private readonly PhysicsWorld physicsWorld;
    private readonly CapsuleCharacter character;
    private readonly PhysicsEntity physicsEntity;
    private readonly ModelEntity entity;
    
    private bool grounded = false;
    private float movementX = 0.0f;
    private bool jump = false;

    public CapsuleCharacterInstance(IInputMethod inputMethod, ResourceManager resourceManager, PhysicsWorld physicsWorld, CapsuleCharacter character)
    {
        this.inputMethod = inputMethod;
        this.physicsWorld = physicsWorld;
        this.character = character;

        var model = resourceManager.Get<Model>("Models/Capsule.dae");
        entity = new ModelEntity(model);
        
        // Initialize physics body
        var capsule = new Capsule(0.5f, 1.0f);
        var capsuleIndex = physicsWorld.Simulation.Shapes.Add(capsule);
        var rigidPose = new RigidPose(System.Numerics.Vector3.Zero, System.Numerics.Quaternion.Identity);
        var bodyDescription = BodyDescription.CreateDynamic(
            rigidPose, 
            new BodyInertia { InverseMass = 1.0f / mass },
            new CollidableDescription(capsuleIndex, 0.1f, float.MaxValue, ContinuousDetection.Passive),
            0.01f);
        physicsEntity = new PhysicsEntity(physicsWorld, bodyDescription);
        
        physicsWorld.Step += OnStep;
    }

    public override float GetAttributeMultiplier(AttributeType type) => 1.0f;

    private void OnStep()
    {
        var bodyReference = physicsEntity.Reference;
        
        // Raycast to check if player is grounded
        var handler = new SimpleRayHitHandler(bodyReference.CollidableReference);
        var rayStart = new Vector3(Position.X, Position.Y, Position.Z);
        physicsWorld.Simulation.RayCast(rayStart.ToSystem(), -System.Numerics.Vector3.UnitY, 1.0f, ref handler);
        grounded = handler.Hit != null;

        // Apply movement
        if (movementX != 0.0f)
        {
            var targetSpeed = grounded 
                ? character.GetAttributeValue(this, AttributeType.GroundSpeed) 
                : character.GetAttributeValue(this, AttributeType.AirSpeed);

            var targetVelocity = Vector3.UnitX * targetSpeed * movementX;
            var currentVelocity = bodyReference.Velocity.Linear.ToOpenTK();
            
            // Zero out Y velocity
            currentVelocity.Y = 0.0f;

            // Calculate force
            var velocity = targetVelocity - currentVelocity;
            velocity /= 1.0f - physicsWorld.Damping;
            var force = velocity * mass;

            bodyReference.Awake = true;
            bodyReference.ApplyLinearImpulse(force.ToSystem()); // Why does this not wake the body?
        }

        // Apply jump
        if (grounded && jump)
        {
            var jumpHeight = character.GetAttributeValue(this, AttributeType.JumpHeight);
            
            // Calculate force
            var velocity = MathF.Sqrt(2.0f * -physicsWorld.Gravity * jumpHeight);
            velocity /= 1.0f - physicsWorld.Damping;
            var force = Vector3.UnitY * velocity * mass;
            
            bodyReference.Awake = true; 
            bodyReference.ApplyLinearImpulse(force.ToSystem());
        }

        // Reset jump
        jump = false;
    }

    public override void Update(UpdateArgs args)
    {
        // Get movement
        movementX = inputMethod.GetMovement().X;

        // Jump
        if (inputMethod.GetJump())
        {
            jump = true;
        }
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
        physicsWorld.Step -= OnStep;
        physicsEntity.Dispose();
    }
}
