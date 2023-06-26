using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Util;
using BepuPhysics;
using FlexFramework.Core;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content.Characters;

public class CharacterController : IUpdateable, IDisposable
{
    public Vector3 Position
    {
        get => smoothPosition;
        set
        {
            physicsEntity.Position = value;
            smoothPosition = value;
            physicsPosition = value;
        }
    }
    
    public Quaternion Rotation
    {
        get => smoothRotation;
        set
        {
            physicsEntity.Rotation = value;
            smoothRotation = value;
            physicsRotation = value;
        }
    }
    
    private readonly Character character;
    private readonly CharacterInstance instance;
    private readonly IInputMethod inputMethod;
    private readonly PhysicsWorld physicsWorld;
    private readonly PhysicsEntity physicsEntity;
    private readonly float mass;
    
    private Vector3 physicsPosition;
    private Quaternion physicsRotation;
    private Vector3 smoothPosition;
    private Quaternion smoothRotation;
    
    private bool grounded = false;
    private bool groundedLastFrame = false; // Frame means physics frame, not game frame
    private float movementX = 0.0f;
    private bool jump = false;
    private int jumpCount = 0;

    public CharacterController(
        Character character, 
        CharacterInstance instance, 
        IInputMethod inputMethod, 
        PhysicsWorld physicsWorld, 
        BodyDescription bodyDescription)
    {
        this.character = character;
        this.instance = instance;
        this.inputMethod = inputMethod;
        this.physicsWorld = physicsWorld;
        
        mass = 1.0f / bodyDescription.LocalInertia.InverseMass;
        physicsEntity = new PhysicsEntity(physicsWorld, bodyDescription);
        physicsWorld.Step += OnStep;
    }
    
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
                ? character.GetAttributeValue(instance, AttributeType.GroundSpeed) 
                : character.GetAttributeValue(instance, AttributeType.AirSpeed);

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
        if (jump && jumpCount < character.GetAttributeValue(instance, AttributeType.JumpCount))
        {
            jumpCount++;
            var jumpHeight = character.GetAttributeValue(instance, AttributeType.JumpHeight);
            
            // Calculate force
            var targetVelocity = Vector3.UnitY * MathF.Sqrt(2.0f * -physicsWorld.Gravity * jumpHeight);
            var currentVelocity = bodyReference.Velocity.Linear.ToOpenTK();
            
            // Zero out X and Z velocity
            currentVelocity.X = 0.0f;
            currentVelocity.Z = 0.0f;
            
            var velocity = targetVelocity - currentVelocity;
            velocity /= 1.0f - physicsWorld.Damping;
            var force = velocity * mass;
            
            bodyReference.Awake = true; 
            bodyReference.ApplyLinearImpulse(force.ToSystem());
        }

        // Reset jump
        jump = false;
        
        // Retrieve physics position and rotation
        physicsPosition = bodyReference.Pose.Position.ToOpenTK();
        physicsRotation = bodyReference.Pose.Orientation.ToOpenTK();
    }

    public void Update(UpdateArgs args)
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


    public void Dispose()
    {
        physicsWorld.Step -= OnStep;
        physicsEntity.Dispose();
    }
}