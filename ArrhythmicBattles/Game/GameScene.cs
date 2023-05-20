﻿using ArrhythmicBattles.Core;
using ArrhythmicBattles.Game.Content;
using BepuPhysics;
using BepuPhysics.Collidables;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.BackgroundRenderers;
using FlexFramework.Core.Rendering.PostProcessing;
using FlexFramework.Physics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Game;

public class GameScene : ABScene
{
    // Entities
    private readonly MapEntity mapEntity;
    private readonly PlayerEntity playerEntity;

    // Other things
    private readonly Bloom bloom;
    private readonly Exposure tonemapper;
    private readonly EdgeDetect edgeDetect;

    private readonly PerspectiveCamera camera;
    private readonly PhysicsWorld physicsWorld;
    private readonly ProceduralSkyboxRenderer skyboxRenderer;
    private readonly ScopedInputProvider inputProvider;
    private DebugScreen? debugScreen;

#if DEBUG
    private ScopedInputProvider? freeCamInputProvider;
    private float freeCamYaw;
    private float freeCamPitch;
#endif

    public GameScene(ABContext context) : base(context)
    {
        Engine.CursorState = CursorState.Grabbed;
        
        // Init resources
        var renderer = Engine.Renderer;
        renderer.ClearColor = Color4.Black;
        if (renderer is ILighting lighting)
        {
            lighting.DirectionalLight =
                new DirectionalLight(new Vector3(0.5f, -1, 0.5f).Normalized(), Vector3.One, 0.7f);
        }

        // Init entities
        mapEntity = EntityManager.Create(() => new MapEntity("Assets/Maps/Playground", Context.Settings));
        
        // We init these here because player entity depends on them
        inputProvider = Context.InputSystem.AcquireInputProvider();
        physicsWorld = new PhysicsWorld(Engine);
        playerEntity = EntityManager.Create(() => new PlayerEntity(inputProvider, physicsWorld, 0.0f, 0.0f));
        playerEntity.Position = Vector3.UnitY * 4.0f;
        
        // Init other things
        skyboxRenderer = new ProceduralSkyboxRenderer();
        
        camera = new PerspectiveCamera();
        camera.DepthFar = 1000.0f;

        // Create floor
        // TODO: Don't hardcode this
        {
            Box floorBox = new Box(20.0f, 0.1f, 20.0f);
            TypedIndex floorShapeIndex = physicsWorld.Simulation.Shapes.Add(floorBox);
            RigidPose floorPose = RigidPose.Identity;
            BodyDescription floorBodyDescription = BodyDescription.CreateKinematic(floorPose, floorShapeIndex, 0.01f);
            physicsWorld.Simulation.Bodies.Add(floorBodyDescription);
        }

        // Init post processing
        bloom = new Bloom();
        tonemapper = new Exposure();
        tonemapper.ExposureValue = 1.2f;
        edgeDetect = new EdgeDetect();
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        // Update physics
        physicsWorld.Update(args);

        if (inputProvider.GetKeyDown(Keys.F3))
        {
            if (debugScreen == null)
            {
                debugScreen = new DebugScreen(Engine, this);
                OpenScreen(debugScreen);
            }
            else
            {
                CloseScreen(debugScreen);
                debugScreen = null;
            }
        }
        
        if (inputProvider.GetKeyDown(Keys.Escape))
        {
            OpenScreen(new PauseScreen(Engine, this));
        }
        
        // Teleport player to origin if they fall off the map
        if (playerEntity.Position.Y < -10.0f)
        {
            playerEntity.Position = Vector3.UnitY * 4.0f;
        }

#if DEBUG
        if (inputProvider.GetKeyDown(Keys.F1))
        {
            // Toggle free cam
            freeCamInputProvider = Context.InputSystem.AcquireInputProvider();
        }
        
        if (freeCamInputProvider != null && freeCamInputProvider.GetKeyDown(Keys.F1))
        {
            freeCamInputProvider.Dispose();
            freeCamInputProvider = null;
        }
#endif
        
#if DEBUG
        // Update camera pos
        if (freeCamInputProvider == null)
        {
#endif
            Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, playerEntity.Yaw) * Quaternion.FromAxisAngle(Vector3.UnitX, playerEntity.Pitch);
            // Vector3 backward = Vector3.Transform(Vector3.UnitZ, rotation);
            // camera.Position = playerEntity.Position + new Vector3(0.0f, 0.75f, 0.0f) + backward * 3.5f;
            // camera.Rotation = rotation;

            Vector3 cameraPos = playerEntity.Position + new Vector3(0.0f, 0.0f, 4.0f);
            camera.Position = Vector3.Lerp(camera.Position, cameraPos, 2.5f * args.DeltaTime);
            camera.Rotation = rotation;
#if DEBUG
        }
        else
        {
            // Free cam
            Vector3 forward = Vector3.Transform(-Vector3.UnitZ, camera.Rotation);
            Vector3 right = Vector3.Transform(Vector3.UnitX, camera.Rotation);
            Vector2 move = freeCamInputProvider.Movement;
            
            Vector3 moveDir = (forward * move.Y + right * move.X) * 5.0f;
            camera.Position += moveDir * args.DeltaTime;
            
            freeCamYaw -= freeCamInputProvider.MouseDelta.X / 480.0f;
            freeCamPitch -= freeCamInputProvider.MouseDelta.Y / 480.0f;
            freeCamPitch = Math.Clamp(freeCamPitch, -MathF.PI * 0.5f, MathF.PI * 0.5f);
            camera.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, freeCamYaw) * Quaternion.FromAxisAngle(Vector3.UnitX, freeCamPitch);
        }
#endif
    }

    protected override void RenderScene(CommandList commandList)
    {
        commandList.AddPostProcessor(bloom);
        commandList.AddPostProcessor(tonemapper);
        commandList.AddPostProcessor(edgeDetect);

        CameraData cameraData = camera.GetCameraData(Engine.ClientSize);
        commandList.UseBackgroundRenderer(skyboxRenderer, cameraData);
        
        RenderArgs alphaClipArgs = new RenderArgs(commandList, LayerType.AlphaClip, MatrixStack, cameraData);
        RenderArgs opaqueArgs = new RenderArgs(commandList, LayerType.Opaque, MatrixStack, cameraData);
        
        // render player
        EntityManager.Invoke(playerEntity, entity => entity.Render(opaqueArgs));
        
        // render map
        EntityManager.Invoke(mapEntity, entity => entity.Render(opaqueArgs));

        // render gui
        CameraData guiCameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        RenderArgs guiArgs = new RenderArgs(commandList, LayerType.Gui, MatrixStack, guiCameraData);
        
        ScreenHandler.Render(guiArgs);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        bloom.Dispose();
        tonemapper.Dispose();
        edgeDetect.Dispose();
        physicsWorld.Dispose();
        skyboxRenderer.Dispose();
        inputProvider.Dispose();
    }
}