using ArrhythmicBattles.MainMenu;
using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Util;
using FlexFramework.Core;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using FlexFramework.Rendering.PostProcessing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainGame;

public class GameScene : Scene
{
    private readonly ABContext context;

    private PerspectiveCamera camera;
    private SkinnedModelEntity modelEntity;
    private ModelEntity envModelEntity;
    private ModelEntity cubeModelEntity;
    private Model model;
    private Model cubeModel;
    private Model envModel;

    private Texture2D skyboxTexture;
    
    private Bloom bloom;
    private Exposure tonemapper;

    private InputSystem inputSystem;
    private InputCapture capture;

    private int alphaClipLayer;

    private float yRotation = 0.0f;
    private float xRotation = 0.0f;

    public GameScene(ABContext context)
    {
        this.context = context;
    }
    
    public override void Init()
    {
        inputSystem = context.InputSystem;
        capture = inputSystem.AcquireCapture();
        
        skyboxTexture = Texture2D.FromExr("skybox", "Assets/Skyboxes/skybox.exr");

        Engine.Renderer.ClearColor = Color4.DeepSkyBlue;
        alphaClipLayer = Engine.Renderer.GetLayerId(DefaultRenderer.AlphaClipLayerName);

        camera = new PerspectiveCamera();
        camera.Position = Vector3.UnitZ * 4.0f;

        model = new Model(@"Assets/Models/WalkAnim.dae");
        model.TextureMinFilter(TextureMinFilter.Nearest);
        model.TextureMagFilter(TextureMagFilter.Nearest);
        modelEntity = new SkinnedModelEntity();
        modelEntity.Model = model;
        modelEntity.Animation = model.Animations[0];

        cubeModel = new Model(@"Assets/Models/Cube.dae");
        cubeModelEntity = new ModelEntity();
        cubeModelEntity.Model = cubeModel;
        cubeModelEntity.Color = new Color4(12.2f, 10.0f, 1.8f, 1.0f);

        envModel = new Model(@"Assets/Models/Environment.dae");
        envModelEntity = new ModelEntity();
        envModelEntity.Model = envModel;
        
        // Init post processing
        bloom = new Bloom();
        tonemapper = new Exposure();
        tonemapper.ExposureValue = 0.8f;
    }

    public override void Update(UpdateArgs args)
    {
        if (Engine.Input.GetKey(Keys.Escape))
        {
            Engine.LoadScene<MainMenuScene>(context);
        }
        
        modelEntity.Update(args);
        envModelEntity.Update(args);
        
        Vector3 forward = Vector3.Transform(-Vector3.UnitZ, camera.Rotation);
        Vector3 right = Vector3.Transform(Vector3.UnitX, camera.Rotation);

        Vector2 movement = inputSystem.GetMovement(capture);

        Vector3 move = forward * movement.Y + right * movement.X;
        camera.Position += move * 6.0f * args.DeltaTime;

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
        renderer.UseSkybox(skyboxTexture);
        renderer.UsePostProcessor(bloom);
        renderer.UsePostProcessor(tonemapper);

        CameraData cameraData = camera.GetCameraData(Engine.ClientSize);
        MatrixStack.Push();
        
        MatrixStack.Push();
        MatrixStack.Translate(0.0f, -0.4f, 8.0f);
        modelEntity.Render(renderer, alphaClipLayer, MatrixStack, cameraData);
        MatrixStack.Pop();
        
        MatrixStack.Push();
        MatrixStack.Translate(0.0f, 4.0f, 0.0f);
        cubeModelEntity.Render(renderer, alphaClipLayer, MatrixStack, cameraData);
        MatrixStack.Pop();
        
        envModelEntity.Render(renderer, alphaClipLayer, MatrixStack, cameraData);
        MatrixStack.Pop();
    }

    public override void Dispose()
    {
        modelEntity.Dispose();
        model.Dispose();
        envModelEntity.Dispose();
        envModel.Dispose();
        capture.Dispose();
        
        bloom.Dispose();
        tonemapper.Dispose();
    }
}