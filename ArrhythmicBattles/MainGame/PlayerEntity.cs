using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Util;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainGame;

public class PlayerEntity : Entity, IRenderable
{
    public Vector3 Position { get; set; } = Vector3.Zero;
    public float Yaw { get; set; } = 0.0f;
    public float Pitch { get; set; } = 0.0f;
    
    private Vector3 velocity = Vector3.Zero;

    private readonly Model model;
    private readonly ModelEntity modelEntity;

    private readonly InputSystem inputSystem;
    private readonly InputInfo inputInfo;

    public PlayerEntity(InputSystem inputSystem, InputInfo inputInfo)
    {
        this.inputSystem = inputSystem;
        this.inputInfo = inputInfo;
        
        model = new Model("Assets/Models/Capsule.dae");
        modelEntity = new ModelEntity();
        modelEntity.Model = model;
    }

    private bool IsGrounded()
    {
        return Position.Y <= 0.5f;
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        // rotation
        Vector2 delta = inputSystem.Input.MouseDelta / 480.0f;
        Yaw -= delta.X;
        Pitch -= delta.Y;
        
        bool grounded = IsGrounded();

        // apply gravity
        velocity.Y -= 9.81f * args.DeltaTime;
        if (grounded)
        {
            velocity.Y = 0.0f;
        }

        // apply movement
        Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, Yaw);
        Vector3 forward = Vector3.Transform(-Vector3.UnitZ, rotation);
        Vector3 right = Vector3.Transform(Vector3.UnitX, rotation);
        Vector2 movement = inputSystem.GetMovement(inputInfo.InputCapture);
        Vector3 move = movement != Vector2.Zero ? Vector3.Normalize(forward * movement.Y + right * movement.X) : Vector3.Zero;
        velocity += move * 0.65f;

        // apply jump
        if (grounded && inputSystem.GetKeyDown(inputInfo.InputCapture, Keys.Space))
        {
            velocity.Y += 5.0f;
        }
        
        // apply friction
        velocity.X *= 0.9f;
        velocity.Z *= 0.9f;

        // apply velocity
        Position += velocity * args.DeltaTime;
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(Position);
        modelEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }
    
    public override void Dispose()
    {
        model.Dispose();
        modelEntity.Dispose();
    }
}