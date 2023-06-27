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
    
    private Vector2 movement = Vector2.Zero;
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
        
        // Reset jump count if just landed
        if (grounded && !groundedLastFrame)
            jumpCount = 0;
        
        groundedLastFrame = grounded;

        // Check to ensure that there is input
        if (movement != Vector2.Zero)
        {
            // Apply movement
            if (movement.X != 0.0f)
            {
                var targetSpeed = grounded 
                    ? character.GetAttributeValue(instance, AttributeType.GroundSpeed) 
                    : character.GetAttributeValue(instance, AttributeType.AirSpeed);

                var targetVelocity = Vector3.UnitX * targetSpeed * movement.X;
                var currentVelocity = bodyReference.Velocity.Linear.ToOpenTK();
            
                // Zero out Y velocity
                currentVelocity.Y = 0.0f;

                // Calculate force
                var velocity = targetVelocity - currentVelocity;
                velocity /= 1.0f - physicsWorld.Damping;
                var force = velocity * mass;
                
                // Limit force using acceleration
                var acceleration = character.GetAttributeValue(instance, AttributeType.Acceleration);
                var maxForce = acceleration * mass;
                if (force.LengthSquared > maxForce * maxForce)
                    force = force.Normalized() * maxForce;
                
                // If force is opposite of velocity, dampen force
                const float oppositeDampingFactor = 0.25f;
                if (Vector3.Dot(force, currentVelocity) < 0.0f)
                    force *= oppositeDampingFactor;
                
                // If body isn't grounded, dampen force
                const float airDampingFactor = 0.25f;
                if (!grounded)
                    force *= airDampingFactor;

                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(force.ToSystem()); // Why does this not wake the body?
            }

            // Apply jump
            if (jump && jumpCount < character.GetAttributeValue(instance, AttributeType.JumpCount))
            {
                jumpCount++;
                var jumpDistance = character.GetAttributeValue(instance, AttributeType.JumpDistance);
                var dotAngle = Vector2.Dot(Vector2.UnitY, movement);
                var angle = MathF.Acos(dotAngle);
                var jumpDirection = !grounded || angle < MathHelper.DegreesToRadians(60.0f) 
                    ? movement // If not grounded or angle is less than 60 degrees, jump in direction of movement
                    : new Vector2(
                        MathF.Cos(MathF.PI / 3.0f) * movement.X < 0.0f ? -1.0f : 1.0f, 
                        MathF.Sin(MathF.PI / 3.0f)) * movement.Length; // Else jump in direction of 60 degrees from horizontal
                var targetVelocity = new Vector3(jumpDirection) * MathF.Sqrt(2.0f * -physicsWorld.Gravity * jumpDistance);
                var currentVelocity = bodyReference.Velocity.Linear.ToOpenTK();
                
                // Calculate force
                var velocity = targetVelocity - currentVelocity;
                velocity /= 1.0f - physicsWorld.Damping;
                var force = velocity * mass;
                
                bodyReference.Awake = true; 
                bodyReference.ApplyLinearImpulse(force.ToSystem());
            }
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
        movement = inputMethod.GetMovement();

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