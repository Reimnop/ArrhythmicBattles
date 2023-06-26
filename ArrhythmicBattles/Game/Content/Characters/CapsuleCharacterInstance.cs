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
    private const float Mass = 40.0f;

    public override Vector3 Position
    {
        get => smoothPosition;
        set
        {
            physicsEntity.Position = value;
            smoothPosition = value;
            physicsPosition = value;
        }
    }
    
    public override Quaternion Rotation
    {
        get => smoothRotation;
        set
        {
            physicsEntity.Rotation = value;
            smoothRotation = value;
            physicsRotation = value;
        }
    }

    private readonly IInputMethod inputMethod;
    private readonly PhysicsWorld physicsWorld;
    private readonly CapsuleCharacter character;
    private readonly PhysicsEntity physicsEntity;
    private readonly ModelEntity entity;
    
    private bool grounded = false;
    private bool groundedLastFrame = false; // Frame means physics frame, not game frame
    private float movementX = 0.0f;
    private bool jump = false;
    private int jumpCount = 0;
    
    private Vector3 physicsPosition;
    private Quaternion physicsRotation;
    
    private Vector3 smoothPosition;
    private Quaternion smoothRotation;

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
            new BodyInertia { InverseMass = 1.0f / Mass },
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
        
        // Reset jump count if grounded
        if (grounded && !groundedLastFrame)
            jumpCount = 0;
        
        groundedLastFrame = grounded;

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
            var force = velocity * Mass;

            bodyReference.Awake = true;
            bodyReference.ApplyLinearImpulse(force.ToSystem()); // Why does this not wake the body?
        }

        // Apply jump
        if (jump && jumpCount < character.GetAttributeValue(this, AttributeType.JumpCount))
        {
            jumpCount++;
            var jumpHeight = character.GetAttributeValue(this, AttributeType.JumpHeight);
            
            // Calculate force
            var targetVelocity = Vector3.UnitY * MathF.Sqrt(2.0f * -physicsWorld.Gravity * jumpHeight);
            var currentVelocity = bodyReference.Velocity.Linear.ToOpenTK();
            
            // Zero out X and Z velocity
            currentVelocity.X = 0.0f;
            currentVelocity.Z = 0.0f;
            
            var velocity = targetVelocity - currentVelocity;
            velocity /= 1.0f - physicsWorld.Damping;
            var force = velocity * Mass;
            
            bodyReference.Awake = true; 
            bodyReference.ApplyLinearImpulse(force.ToSystem());
        }

        // Reset jump
        jump = false;
        
        // Retrieve physics position and rotation
        physicsPosition = bodyReference.Pose.Position.ToOpenTK();
        physicsRotation = bodyReference.Pose.Orientation.ToOpenTK();
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
        
        // Smooth physics position and rotation
        var t = MathHelper.Clamp(args.DeltaTime / physicsWorld.TimeStep, 0.0f, 1.0f);
        smoothPosition = Vector3.Lerp(smoothPosition, physicsPosition, t);
        smoothRotation = Quaternion.Slerp(smoothRotation, physicsRotation, t);
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
