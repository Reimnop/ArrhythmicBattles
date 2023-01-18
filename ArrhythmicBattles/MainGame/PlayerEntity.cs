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
        public bool HitFound { get; private set; } = false;

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
            HitFound = true;
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
    private readonly PhysicsManager physicsManager;

    private readonly BodyHandle bodyHandle;
    
    private bool grounded = false;

    public PlayerEntity(InputSystem inputSystem, InputInfo inputInfo, PhysicsManager physicsManager, Vector3 position, float yaw, float pitch)
    {
        this.inputSystem = inputSystem;
        this.inputInfo = inputInfo;
        this.physicsManager = physicsManager;
        this.position = position;
        this.yaw = yaw;
        this.pitch = pitch;
        
        model = new Model("Assets/Models/Capsule.dae");
        modelEntity = new ModelEntity();
        modelEntity.Model = model;
        
        // create shape
        Capsule capsule = new Capsule(0.5f, 1.0f);
        TypedIndex capsuleIndex = physicsManager.Simulation.Shapes.Add(capsule);
        RigidPose rigidPose = new RigidPose(position.ToSystem(), Quaternion.FromAxisAngle(Vector3.UnitY, yaw).ToSystem());
        BodyDescription bodyDescription = BodyDescription.CreateDynamic(
            rigidPose, 
            new BodyInertia { InverseMass = 1.0f / 10.0f },
            new CollidableDescription(capsuleIndex, 0.1f, float.MaxValue, ContinuousDetection.Passive),
            0.01f);
        bodyHandle = physicsManager.Simulation.Bodies.Add(bodyDescription);

        // add to physics manager
        physicsManager.Step += OnStep;
    }

    private void OnStep()
    {
        // raycast to check if player is grounded
        BodyReference bodyReference = physicsManager.Simulation.Bodies.GetBodyReference(bodyHandle);
        RayHitHandler handler = new RayHitHandler(bodyReference.CollidableReference);
        Vector3 rayStart = new Vector3(position.X, position.Y - 1.0f, position.Z);
        physicsManager.Simulation.RayCast(rayStart.ToSystem(), -System.Numerics.Vector3.UnitY, 0.5f, ref handler);
        grounded = handler.HitFound;
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);

        BodyReference bodyReference = physicsManager.Simulation.Bodies.GetBodyReference(bodyHandle);
        
        // apply movement
        Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, Yaw);
        Vector3 forward = Vector3.Transform(-Vector3.UnitZ, rotation);
        Vector3 right = Vector3.Transform(Vector3.UnitX, rotation);
        Vector2 movement = inputSystem.GetMovement(inputInfo.InputCapture);
        Vector3 move = movement != Vector2.Zero ? Vector3.Normalize(forward * movement.Y + right * movement.X) : Vector3.Zero;
        Vector2 targetVelocity = new Vector2(move.X, move.Z) * 6.0f;
        
        if (targetVelocity != Vector2.Zero)
        {
            Vector2 currentVelocity = new Vector2(bodyReference.Velocity.Linear.X, bodyReference.Velocity.Linear.Z);
            Vector2 velocityDir = targetVelocity - currentVelocity;
            Vector3 force = new Vector3(velocityDir.X, 0.0f, velocityDir.Y) * 5.0f;
            bodyReference.ApplyLinearImpulse(force.ToSystem());
        }

        // apply jump
        if (grounded && inputSystem.GetKeyDown(inputInfo.InputCapture, Keys.Space))
        {
            bodyReference.ApplyLinearImpulse(new Vector3(0.0f, 50.0f, 0.0f).ToSystem());
        }
        
        // update position
        position = bodyReference.Pose.Position.ToOpenTK();

        // camera rotation
        Vector2 delta = inputSystem.Input.MouseDelta / 480.0f;
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
        
        physicsManager.Step -= OnStep;
    }
}