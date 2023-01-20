using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Util;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Util;
using FlexFramework.Physics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainGame;

public class PlayerEntity : Entity, IRenderable
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
            return true;
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

    private readonly InputSystem inputSystem;
    private readonly InputInfo inputInfo;
    private readonly PhysicsWorld physicsWorld;

    private readonly BodyHandle bodyHandle;
    
    private bool grounded = false;
    
    private Vector2 movement = Vector2.Zero;
    private bool jump = false;

    public PlayerEntity(InputSystem inputSystem, InputInfo inputInfo, PhysicsWorld physicsWorld, Vector3 position, float yaw, float pitch)
    {
        this.inputSystem = inputSystem;
        this.inputInfo = inputInfo;
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
            new BodyInertia { InverseMass = 1.0f / 70.0f },
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
        Vector3 rayStart = new Vector3(position.X, position.Y - 0.8f, position.Z);
        physicsWorld.Simulation.RayCast(rayStart.ToSystem(), -System.Numerics.Vector3.UnitY, 0.7f, ref handler);
        grounded = handler.Hit != null;

        // apply movement
        if (movement != Vector2.Zero)
        {
            Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, Yaw);
            Vector3 forward = Vector3.Transform(-Vector3.UnitZ, rotation);
            Vector3 right = Vector3.Transform(Vector3.UnitX, rotation);
            Vector3 direction = forward * movement.Y + right * movement.X;
            Vector2 move = new Vector2(direction.X, direction.Z).Normalized();
            Vector3 velocity = bodyReference.Velocity.Linear.ToOpenTK();
            velocity.X = move.X * 6.0f;
            velocity.Z = move.Y * 6.0f;
            bodyReference.Velocity.Linear = velocity.ToSystem();
        }

        // apply jump
        if (grounded && jump)
        {
            bodyReference.ApplyLinearImpulse(new System.Numerics.Vector3(0.0f, 350.0f, 0.0f));
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
        movement = inputSystem.GetMovement(inputInfo.InputCapture);

        // jump
        if (inputSystem.GetKeyDown(inputInfo.InputCapture, Keys.Space))
        {
            jump = true;
        }

        // camera rotation
        Vector2 delta = inputSystem.GetMouseDelta(inputInfo.InputCapture) / 480.0f;
        yaw -= delta.X;
        pitch = Math.Clamp(pitch - delta.Y, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(position);
        modelEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }
    
    public override void Dispose()
    {
        modelEntity.Dispose();

        physicsWorld.Step -= OnStep;
    }
}