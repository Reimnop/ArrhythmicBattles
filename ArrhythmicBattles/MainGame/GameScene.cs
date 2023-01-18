﻿using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Util;
using BepuPhysics;
using BepuPhysics.Collidables;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.PostProcessing;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainGame;

public class GameScene : ABScene
{
    private PlayerEntity playerEntity = null!;
    
    private PerspectiveCamera camera = null!;
    private ModelEntity envModelEntity = null!;
    private Model envModel = null!;
    private Model capsuleModel = null!;

    private List<PhysicsEntity> physicsEntities = new List<PhysicsEntity>();

    private Texture2D skyboxTexture = null!;
    
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
        
        skyboxTexture = Texture2D.FromExr("skybox", "Assets/Skyboxes/skybox.exr");
        Engine.Renderer.ClearColor = Color4.Black;
        
        envModel = new Model(@"Assets/Models/Map01.dae");
        envModelEntity = new ModelEntity();
        envModelEntity.Model = envModel;
        envModel.Materials.First(x => x.Name == "Highlight").EmissiveStrength = 4.0f;
        
        opaqueLayer = Engine.Renderer.GetLayerId(DefaultRenderer.OpaqueLayerName);
        alphaClipLayer = Engine.Renderer.GetLayerId(DefaultRenderer.AlphaClipLayerName);

        camera = new PerspectiveCamera();
        camera.DepthFar = 1000.0f;
        camera.Position = Vector3.UnitZ * 4.0f;
        
        playerEntity = new PlayerEntity(inputSystem, inputInfo, PhysicsManager, Vector3.UnitY * 4.0f, 0.0f, 0.0f);

        // Create floor
        Box floorBox = new Box(20.0f, 0.1f, 20.0f);
        TypedIndex floorShapeIndex = PhysicsManager.Simulation.Shapes.Add(floorBox);
        RigidPose floorPose = RigidPose.Identity;
        BodyDescription floorBodyDescription = BodyDescription.CreateKinematic(floorPose, floorShapeIndex, 0.01f);
        PhysicsManager.Simulation.Bodies.Add(floorBodyDescription);

        // Spawn a bunch of physicsEntities
        capsuleModel = new Model("Assets/Models/Capsule.dae");

        Capsule capsule = new Capsule(0.25f, 0.5f);
        TypedIndex capsuleShape = PhysicsManager.Simulation.Shapes.Add(capsule);
        BodyInertia inertia = capsule.ComputeInertia(1.0f);

        Random random = new Random(2);
        for (int i = 0; i < 2400; i++)
        {
            Vector3 position = new Vector3(
                random.NextSingle() * 10.0f - 5.0f,
                random.NextSingle() * 100.0f + 250.0f,
                random.NextSingle() * 10.0f - 5.0f);
            
            Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitX, random.NextSingle() * MathF.PI * 2.0f);
            
            PhysicsEntity physicsEntity = new PhysicsEntity(PhysicsManager.Simulation, capsuleModel, capsuleShape, inertia, position, rotation);
            physicsEntities.Add(physicsEntity);
        }

        // Init post processing
        bloom = new Bloom();
        tonemapper = new Exposure();
        tonemapper.ExposureValue = 1.2f;
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);

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
    
        playerEntity.Update(args);
        envModelEntity.Update(args);
        
        camera.Position = playerEntity.Position + Vector3.UnitY * 0.65f;
        camera.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, playerEntity.Yaw) * 
                          Quaternion.FromAxisAngle(Vector3.UnitX, playerEntity.Pitch);

        foreach (PhysicsEntity physicsEntity in physicsEntities)
        {
            physicsEntity.Update(args);
        }
    }

    public override void Render(Renderer renderer)
    {
        renderer.UsePostProcessor(bloom);
        renderer.UsePostProcessor(tonemapper);

        CameraData cameraData = camera.GetCameraData(Engine.ClientSize);
        renderer.UseSkybox(skyboxTexture, cameraData);
        
        // render player
        playerEntity.Render(renderer, opaqueLayer, MatrixStack, cameraData);
        
        // render physicsEntities
        foreach (PhysicsEntity capsule in physicsEntities)
        {
            capsule.Render(renderer, opaqueLayer, MatrixStack, cameraData);
        }
        
        // render environment
        MatrixStack.Push();
        envModelEntity.Render(renderer, alphaClipLayer, MatrixStack, cameraData);
        MatrixStack.Pop();

        CameraData guiCameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        ScreenHandler.Render(renderer, GuiLayerId, MatrixStack, guiCameraData);
    }

    public override void Dispose()
    {
        base.Dispose();

        playerEntity.Dispose();
        envModelEntity.Dispose();
        
        foreach (PhysicsEntity capsule in physicsEntities)
        {
            capsule.Dispose();
        }
        
        capsuleModel.Dispose();
        envModel.Dispose();
        inputInfo.Dispose();
        
        bloom.Dispose();
        tonemapper.Dispose();
    }
}