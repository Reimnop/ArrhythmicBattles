using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Core;
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
    // Resources
    private readonly Model envModel;
    
    // Entities
    private readonly PlayerEntity playerEntity;
    private readonly ModelEntity envModelEntity;
    
    // Other things
    private readonly PerspectiveCamera camera;
    private readonly PhysicsWorld physicsWorld;
    private readonly ProceduralSkyboxRenderer skyboxRenderer;
    private readonly Bloom bloom;
    private readonly Exposure tonemapper;
    private readonly ScopedInputProvider inputProvider;
    private DebugScreen? debugScreen;
    private CommandList commandList = new();

#if DEBUG
    private SkinnedModelEntity? testModelEntity;
    private Model? testModel;
    
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
        
        envModel = new Model(@"Assets/Models/Map01.dae");
        inputProvider = Context.InputSystem.AcquireInputProvider();
        physicsWorld = new PhysicsWorld(Engine);
        skyboxRenderer = new ProceduralSkyboxRenderer();
        
        // Init entities
        envModelEntity = CreateEntity(() => new ModelEntity(envModel));
        playerEntity = CreateEntity(() => new PlayerEntity(inputProvider, physicsWorld, Vector3.UnitY * 4.0f, 0.0f, 0.0f));
        
        // Init other things
        camera = new PerspectiveCamera();
        camera.DepthFar = 1000.0f;

        // Create floor
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

#if DEBUG
        const string testModelPath = @"Assets/Test/test.fbx";

        if (File.Exists(testModelPath))
        {
            testModel = new Model(testModelPath);
            ModelAnimation testAnimation = testModel.Animations[0];

            testModelEntity = CreateEntity(() => new SkinnedModelEntity(testModel));
            testModelEntity.AnimationHandler.Transition(testAnimation);
        }
#endif
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

    public override void Render(Renderer renderer)
    {
        commandList.Clear();
        
        commandList.AddPostProcessor(bloom);
        commandList.AddPostProcessor(tonemapper);

        CameraData cameraData = camera.GetCameraData(Engine.ClientSize);
        commandList.UseBackgroundRenderer(skyboxRenderer, cameraData);
        
        RenderArgs alphaClipArgs = new RenderArgs(commandList, LayerType.AlphaClip, MatrixStack, cameraData);
        RenderArgs opaqueArgs = new RenderArgs(commandList, LayerType.Opaque, MatrixStack, cameraData);
        
        // render player
        EntityCall(playerEntity, entity => entity.Render(opaqueArgs));

        // render environment
        MatrixStack.Push();
        EntityCall(envModelEntity, entity => entity.Render(alphaClipArgs));

#if DEBUG
        if (testModelEntity != null)
        {
            // render test model
            MatrixStack.Push();
            EntityCall(testModelEntity, entity => entity.Render(alphaClipArgs));
            MatrixStack.Pop();
        }
#endif

        MatrixStack.Pop();

        // render gui
        CameraData guiCameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        RenderArgs guiArgs = new RenderArgs(commandList, LayerType.Gui, MatrixStack, guiCameraData);
        
        ScreenHandler.Render(guiArgs);
        
        // Render scene
        renderer.Render(Engine.ClientSize, commandList, Context.RenderBuffer);
        Engine.Present(Context.RenderBuffer);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        envModel.Dispose();
        physicsWorld.Dispose();
        skyboxRenderer.Dispose();
        bloom.Dispose();
        tonemapper.Dispose();
        inputProvider.Dispose();
    }
}