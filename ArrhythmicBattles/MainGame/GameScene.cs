using ArrhythmicBattles.MainMenu;
using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.PostProcessing;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainGame;

public class GameScene : ABScene
{
    private PerspectiveCamera camera;
    private ModelEntity envModelEntity;
    private Model envModel;

    private Texture2D skyboxTexture;
    
    private Bloom bloom;
    private Exposure tonemapper;

    private InputSystem inputSystem;
    private InputCapture capture;

    private int alphaClipLayer;

    private float yRotation = 0.0f;
    private float xRotation = 0.0f;

    public GameScene(ABContext context) : base(context)
    {
    }
    
    public override void Init()
    {
        base.Init();
        
        inputSystem = Context.InputSystem;
        capture = inputSystem.AcquireCapture();
        
        skyboxTexture = Texture2D.FromExr("skybox", "Assets/Skyboxes/skybox.exr");
        Engine.Renderer.ClearColor = Color4.Black;
        alphaClipLayer = Engine.Renderer.GetLayerId(DefaultRenderer.AlphaClipLayerName);

        camera = new PerspectiveCamera();
        camera.Position = Vector3.UnitZ * 4.0f;

        envModel = new Model(@"Assets/Models/Map01.dae");
        envModelEntity = new ModelEntity();
        envModelEntity.Model = envModel;
        envModel.Materials.First(x => x.Name == "Highlight").EmissiveStrength = 4.0f;
        
        // Init post processing
        bloom = new Bloom();
        tonemapper = new Exposure();
        tonemapper.ExposureValue = 1.2f;
    }

    public override void SetScreen(Screen? screen)
    {
        throw new NotImplementedException();
    }

    public override void Update(UpdateArgs args)
    {
        if (Engine.Input.GetKey(Keys.Escape))
        {
            Engine.LoadScene<MainMenuScene>(Context);
        }
        
        envModelEntity.Update(args);
        
        Vector3 forward = Vector3.Transform(-Vector3.UnitZ, camera.Rotation);
        Vector3 right = Vector3.Transform(Vector3.UnitX, camera.Rotation);

        Vector2 movement = inputSystem.GetMovement(capture);

        Vector3 move = forward * movement.Y + right * movement.X;
        camera.Position += move * 4.0f * args.DeltaTime;

        if (inputSystem.GetMouse(capture, MouseButton.Right))
        {
            Vector2 delta = Engine.Input.MouseDelta / 480.0f;

            yRotation -= delta.X;
            xRotation -= delta.Y;

            camera.Rotation =
                Quaternion.FromAxisAngle(Vector3.UnitY, yRotation) * 
                Quaternion.FromAxisAngle(Vector3.UnitX, xRotation);
        }
    }

    public override void Render(Renderer renderer)
    {
        renderer.UsePostProcessor(bloom);
        renderer.UsePostProcessor(tonemapper);

        CameraData cameraData = camera.GetCameraData(Engine.ClientSize);
        // renderer.UseSkybox(skyboxTexture, cameraData);
        
        MatrixStack.Push();
        envModelEntity.Render(renderer, alphaClipLayer, MatrixStack, cameraData);
        MatrixStack.Pop();
    }

    public override void Dispose()
    {
        envModelEntity.Dispose();
        envModel.Dispose();
        capture.Dispose();
        
        bloom.Dispose();
        tonemapper.Dispose();
    }
}