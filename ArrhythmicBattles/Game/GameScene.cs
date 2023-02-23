using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Util;
using BepuPhysics;
using BepuPhysics.Collidables;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.BackgroundRenderers;
using FlexFramework.Core.Rendering.PostProcessing;
using FlexFramework.Physics;
using OpenTK.Mathematics;
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

    private InputSystem inputSystem = null!;
    private InputInfo inputInfo = null!;

    private DebugScreen? debugScreen;

    private int opaqueLayer;
    private int alphaClipLayer;

    public GameScene(ABContext context) : base(context)
    {
    }
    
    public override void Init()
    {
        base.Init();
        
        inputSystem = Context.InputSystem;
        inputInfo = inputSystem.GetInputInfo();
        
        physicsWorld = new PhysicsWorld(Engine);
        
        skyboxRenderer = new ProceduralSkyboxRenderer();

        Renderer renderer = Engine.Renderer;
        
        renderer.ClearColor = Color4.Black;
        if (renderer is ILighting lighting)
        {
            lighting.DirectionalLight = new DirectionalLight(new Vector3(0.5f, -1, 0.5f).Normalized(), Vector3.One, 0.7f);
        }
        
        envModel = new Model(@"Assets/Models/Map01.dae");
        envModelEntity = new ModelEntity();
        envModelEntity.Model = envModel;
        envModel.Materials.First(x => x.Name == "Highlight").EmissiveStrength = 4.0f;
        
        opaqueLayer = Engine.Renderer.GetLayerId(DefaultRenderer.OpaqueLayerName);
        alphaClipLayer = Engine.Renderer.GetLayerId(DefaultRenderer.AlphaClipLayerName);

        camera = new PerspectiveCamera();
        camera.DepthFar = 1000.0f;
        camera.Position = Vector3.UnitZ * 4.0f;
        
        playerEntity = new PlayerEntity(inputSystem, inputInfo, physicsWorld, Vector3.UnitY * 4.0f, 0.0f, 0.0f);

        // Create floor
        Box floorBox = new Box(20.0f, 0.1f, 20.0f);
        TypedIndex floorShapeIndex = physicsWorld.Simulation.Shapes.Add(floorBox);
        RigidPose floorPose = RigidPose.Identity;
        BodyDescription floorBodyDescription = BodyDescription.CreateKinematic(floorPose, floorShapeIndex, 0.01f);
        physicsWorld.Simulation.Bodies.Add(floorBodyDescription);

        // Init post processing
        bloom = new Bloom();
        tonemapper = new Exposure();
        tonemapper.ExposureValue = 1.2f;
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        physicsWorld.Update(args);
        playerEntity.Update(args);

        if (inputSystem.GetKeyDown(inputInfo.InputCapture, Keys.F3))
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
        
        if (inputSystem.GetKeyDown(inputInfo.InputCapture, Keys.Escape))
        {
            OpenScreen(new PauseScreen(Engine, this));
        }
        
        envModelEntity.Update(args);
        
        // update camera
        Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, playerEntity.Yaw) * 
                              Quaternion.FromAxisAngle(Vector3.UnitX, playerEntity.Pitch);
        
        Vector3 backward = Vector3.Transform(Vector3.UnitZ, rotation);

        camera.Position = playerEntity.Position + new Vector3(0.0f, 0.75f, 0.0f) + backward * 3.5f;
        camera.Rotation = rotation;
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
        MatrixStack.Pop();

        // render gui
        CameraData guiCameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        RenderArgs guiArgs = new RenderArgs(renderer, GuiLayerId, MatrixStack, guiCameraData);
        
        ScreenHandler.Render(guiArgs);
    }

    public override void Dispose()
    {
        base.Dispose();

        playerEntity.Dispose();
        physicsWorld.Dispose();
        
        envModel.Dispose();
        inputInfo.Dispose();
        
        bloom.Dispose();
        tonemapper.Dispose();
        
        skyboxRenderer.Dispose();
    }
}