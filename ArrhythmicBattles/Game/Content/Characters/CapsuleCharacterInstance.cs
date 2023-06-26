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
    private struct RayHitHandler : IRayHitHandler
    {
        public CollidableReference? Hit { get; private set; } = null;

        private readonly CollidableReference collidable;
        
        public RayHitHandler(CollidableReference collidable)
        {
            this.collidable = collidable;
        }
        
        public bool AllowTest(CollidableReference collidable)
        {
            return collidable != this.collidable;
        }

        public bool AllowTest(CollidableReference collidable, int childIndex)
        {
            return collidable != this.collidable;
        }

        public void OnRayHit(in RayData ray, ref float maximumT, float t, in System.Numerics.Vector3 normal, CollidableReference collidable, int childIndex)
        {
            Hit = collidable;
        }
    }

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
            new BodyInertia { InverseMass = 1.0f / 40.0f },
            new CollidableDescription(capsuleIndex, 0.1f, float.MaxValue, ContinuousDetection.Passive),
            0.01f);
        physicsEntity = new PhysicsEntity(physicsWorld, bodyDescription);
        
        physicsWorld.Step += OnStep;
    }

    public override float GetAttributeMultiplier(AttributeType type) => 1.0f;

    private void OnStep()
    {
        const float dragCoefficient = 0.15f;
        
        var bodyReference = physicsEntity.Reference;
        
        // Raycast to check if player is grounded
        var handler = new RayHitHandler(bodyReference.CollidableReference);
        var rayStart = new Vector3(Position.X, Position.Y, Position.Z);
        physicsWorld.Simulation.RayCast(rayStart.ToSystem(), -System.Numerics.Vector3.UnitY, 1.0f, ref handler);
        grounded = handler.Hit != null;

        // Apply movement
        if (movementX != 0.0f)
        {
            // Get speed values
            var targetSpeed = grounded 
                ? character.GetAttributeValue(this, AttributeType.GroundSpeed) 
                : character.GetAttributeValue(this, AttributeType.AirSpeed);

            var currentSpeed = bodyReference.Velocity.Linear.Length();
            
            // Calculate force from speed and drag coefficient
            var force = Vector3.UnitX * movementX * ((targetSpeed - currentSpeed) / dragCoefficient);

            bodyReference.Awake = true;
            bodyReference.ApplyLinearImpulse(force.ToSystem()); // Why does this not wake the body?
        }

        // Apply jump
        if (grounded && jump)
        {
            bodyReference.Awake = true; 
            bodyReference.ApplyLinearImpulse(new System.Numerics.Vector3(0.0f, 500.0f, 0.0f));
        }
        
        // Apply drag
        var velocity = bodyReference.Velocity.Linear.ToOpenTK();
        if (velocity != Vector3.Zero)
        {
            var velocityDirection = velocity.Normalized();
            var drag = velocityDirection * -dragCoefficient * velocity.LengthSquared;
            bodyReference.ApplyLinearImpulse(drag.ToSystem());
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
