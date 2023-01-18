﻿using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Util;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Util;
using FlexFramework.Physics;
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
    private readonly PhysicsManager physicsManager;

    public PlayerEntity(InputSystem inputSystem, InputInfo inputInfo, PhysicsManager physicsManager)
    {
        this.inputSystem = inputSystem;
        this.inputInfo = inputInfo;
        this.physicsManager = physicsManager;
        
        model = new Model("Assets/Models/Capsule.dae");
        modelEntity = new ModelEntity();
        modelEntity.Model = model;

        physicsManager.Step += OnStep;
    }

    private bool IsGrounded()
    {
        Vector3 groundMin = new Vector3(-10.0f, -0.1f, -10.0f);
        Vector3 groundMax = new Vector3(10.0f, 0.1f, 10.0f);
        Box3 groundBox = new Box3(groundMin, groundMax);
        
        Vector3 playerMin = new Vector3(Position.X - 0.5f, Position.Y, Position.Z - 0.5f);
        Vector3 playerMax = new Vector3(Position.X + 0.5f, Position.Y + 2.0f, Position.Z + 0.5f);
        Box3 playerBox = new Box3(playerMin, playerMax);
        
        return groundBox.Contains(playerBox);
    }

    private void OnStep()
    {
        bool grounded = IsGrounded();

        // apply gravity
        velocity.Y -= 9.81f * physicsManager.TimeStep;
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
        if (grounded && inputSystem.GetKey(inputInfo.InputCapture, Keys.Space))
        {
            velocity.Y += 5.0f;
        }
        
        // apply friction
        velocity.X *= 0.9f;
        velocity.Z *= 0.9f;

        // apply velocity
        Position += velocity * physicsManager.TimeStep;
        
        // if y is below -10, reset position and velocity
        if (Position.Y < -10.0f)
        {
            Position = new Vector3(0.0f, 4.0f, 0.0f);
            velocity = Vector3.Zero;
        }
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        // camera rotation
        Vector2 delta = inputSystem.Input.MouseDelta / 480.0f;
        Yaw -= delta.X;
        Pitch -= delta.Y;
        
        Pitch = Math.Clamp(Pitch, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y + 1.0f, Position.Z);
        modelEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }
    
    public override void Dispose()
    {
        modelEntity.Dispose();
        
        physicsManager.Step -= OnStep;
    }
}