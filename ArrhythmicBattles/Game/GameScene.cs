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
    private PlayerEntity playerEntity = null!;
    
    private PerspectiveCamera camera = null!;
    private ModelEntity envModelEntity = null!;
    private Model envModel = null!;

    private PhysicsWorld physicsWorld = null!;

    private ProceduralSkyboxRenderer skyboxRenderer = null!;
    
    private Bloom bloom = null!;
    private Exposure tonemapper = null!;
    
    private ScopedInputProvider inputProvider = null!;

    private DebugScreen? debugScreen;

    private int opaqueLayer;
    private int alphaClipLayer;

#if DEBUG
    private ModelEntity? testModelEntity;
    private Model? testModel;
    
    private ScopedInputProvider? freeCamInputProvider;
    private float freeCamYaw;
    private float freeCamPitch;
#endif
    
    public GameScene(ABContext context) : base(context)
    {
    }
    
    public override void Init()
    {
        base.Init();
        
        Engine.CursorState = CursorState.Grabbed;
        
        inputProvider = Context.InputSystem.AcquireInputProvider();
        RegisterObject(inputProvider);
        
        physicsWorld = new PhysicsWorld(Engine);
        RegisterObject(physicsWorld);
        
        skyboxRenderer = new ProceduralSkyboxRenderer();
        RegisterObject(skyboxRenderer);

        Renderer renderer = Engine.Renderer;
        
        renderer.ClearColor = Color4.Black;
        if (renderer is ILighting lighting)
        {
            lighting.DirectionalLight = new DirectionalLight(new Vector3(0.5f, -1, 0.5f).Normalized(), Vector3.One, 0.7f);
        }
        
        envModel = new Model(@"Assets/Models/Map01.dae");
        envModel.Materials.First(x => x.Name == "Highlight").EmissiveStrength = 4.0f;
        RegisterObject(envModel);

        envModelEntity = new ModelEntity();
        envModelEntity.Model = envModel;
        RegisterObject(envModelEntity);

#if DEBUG
        const string testModelPath = @"Assets/test.dae";

        if (File.Exists(testModelPath))
        {
            testModel = new Model(testModelPath);
            RegisterObject(testModel);
        
            testModelEntity = new ModelEntity();
            testModelEntity.Model = testModel;
            RegisterObject(testModelEntity);
        }
#endif

        opaqueLayer = Engine.Renderer.GetLayerId(DefaultRenderer.OpaqueLayerName);
        alphaClipLayer = Engine.Renderer.GetLayerId(DefaultRenderer.AlphaClipLayerName);

        camera = new PerspectiveCamera();
        camera.DepthFar = 1000.0f;

        playerEntity = new PlayerEntity(inputProvider, physicsWorld, Vector3.UnitY * 4.0f, 0.0f, 0.0f);
        RegisterObject(playerEntity);

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
        RegisterObject(bloom);
        
        tonemapper = new Exposure();
        tonemapper.ExposureValue = 1.2f;
        RegisterObject(tonemapper);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);

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

            Vector3 cameraPos = playerEntity.Position + new Vector3(0.0f, 1.0f, 4.0f);
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
        renderer.UsePostProcessor(bloom);
        renderer.UsePostProcessor(tonemapper);

        CameraData cameraData = camera.GetCameraData(Engine.ClientSize);
        renderer.UseBackgroundRenderer(skyboxRenderer, cameraData);
        
        RenderArgs alphaClipArgs = new RenderArgs(renderer, alphaClipLayer, MatrixStack, cameraData);
        RenderArgs opaqueArgs = new RenderArgs(renderer, opaqueLayer, MatrixStack, cameraData);
        
        // render player
        playerEntity.Render(opaqueArgs);

        // render environment
        MatrixStack.Push();
        envModelEntity.Render(alphaClipArgs);

#if DEBUG
        if (testModelEntity != null)
        {
            // render test model
            MatrixStack.Push();
            MatrixStack.Translate(0.0f, 0.0f, 0.0f);
            testModelEntity.Render(alphaClipArgs);
            MatrixStack.Pop();
        }
#endif

        MatrixStack.Pop();

        // render gui
        CameraData guiCameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        RenderArgs guiArgs = new RenderArgs(renderer, GuiLayerId, MatrixStack, guiCameraData);
        
        ScreenHandler.Render(guiArgs);
    }
}