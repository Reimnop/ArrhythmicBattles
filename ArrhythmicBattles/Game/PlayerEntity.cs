using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Util;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using FlexFramework.Physics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Game;

public class PlayerEntity : Entity, IRenderable, IDisposable
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
    
    public Vector3 Position => position;
    public float Yaw => yaw;
    public float Pitch => pitch;
    
    private Vector3 position = Vector3.Zero;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private readonly Model model;
    private readonly ModelEntity modelEntity;

    private readonly IInputProvider inputProvider;
    private readonly PhysicsWorld physicsWorld;

    private readonly BodyHandle bodyHandle;
    
    private bool grounded = false;
    
    private Vector2 movement = Vector2.Zero;
    private bool jump = false;

    public PlayerEntity(IInputProvider inputProvider, PhysicsWorld physicsWorld, Vector3 position, float yaw, float pitch)
    {
        this.inputProvider = inputProvider;
        this.physicsWorld = physicsWorld;
        this.position = position;
        this.yaw = yaw;
        this.pitch = pitch;
        
        model = new Model("Assets/Models/Capsule.dae");
        modelEntity = new ModelEntity();
        modelEntity.Model = model;
        
        // create shape
        Capsule capsule = new Capsule(0.5f, 1.0f);
        TypedIndex capsuleIndex = physicsWorld.Simulation.Shapes.Add(capsule);
        RigidPose rigidPose = new RigidPose(position.ToSystem(), Quaternion.FromAxisAngle(Vector3.UnitY, yaw).ToSystem());
        BodyDescription bodyDescription = BodyDescription.CreateDynamic(
            rigidPose, 
            new BodyInertia { InverseMass = 1.0f / 40.0f },
            new CollidableDescription(capsuleIndex, 0.1f, float.MaxValue, ContinuousDetection.Passive),
            0.01f);
        bodyHandle = physicsWorld.Simulation.Bodies.Add(bodyDescription);

        // add to physics manager
        physicsWorld.Step += OnStep;
    }

    private void OnStep()
    {
        BodyReference bodyReference = physicsWorld.Simulation.Bodies.GetBodyReference(bodyHandle);
        
        // raycast to check if player is grounded
        RayHitHandler handler = new RayHitHandler(bodyReference.CollidableReference);
        Vector3 rayStart = new Vector3(position.X, position.Y, position.Z);
        physicsWorld.Simulation.RayCast(rayStart.ToSystem(), -System.Numerics.Vector3.UnitY, 1.0f, ref handler);
        grounded = handler.Hit != null;

        // apply movement
        if (movement != Vector2.Zero)
        {
            Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, Yaw);
            Vector3 forward = Vector3.Transform(-Vector3.UnitZ, rotation);
            Vector3 right = Vector3.Transform(Vector3.UnitX, rotation);
            Vector3 direction = forward * movement.Y + right * movement.X;
            Vector2 move = new Vector2(direction.X, direction.Z).Normalized();
            Vector3 force = new Vector3(move.X, 0.0f, move.Y) * (grounded ? 15.0f : 8.0f);
            
            bodyReference.Awake = true;
            bodyReference.ApplyLinearImpulse(force.ToSystem()); // why does this not wake the body?
        }

        // apply jump
        if (grounded && jump)
        {
            bodyReference.Awake = true; 
            bodyReference.ApplyLinearImpulse(new System.Numerics.Vector3(0.0f, 250.0f, 0.0f));
        }
        
        Vector3 velocity = bodyReference.Velocity.Linear.ToOpenTK();
        if (velocity != Vector3.Zero)
        {
            const float dragCoefficient = 0.15f;
            
            Vector3 velocityDirection = velocity.Normalized();
            Vector3 drag = velocityDirection * -dragCoefficient * velocity.LengthSquared;
            bodyReference.ApplyLinearImpulse(drag.ToSystem());
        }
        
        // reset jump
        jump = false;
        
        // update position
        position = bodyReference.Pose.Position.ToOpenTK();
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        // get movement
        movement = inputProvider.Movement;

        // jump
        if (inputProvider.GetKeyDown(Keys.Space))
        {
            jump = true;
        }

        // camera rotation
        // Vector2 delta = inputProvider.MouseDelta / 480.0f;
        // yaw -= delta.X;
        // pitch = Math.Clamp(pitch - delta.Y, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
    }

    public void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(position);
        modelEntity.Render(args);
        matrixStack.Pop();
    }
    
    public void Dispose()
    {
        physicsWorld.Step -= OnStep;
    }
}