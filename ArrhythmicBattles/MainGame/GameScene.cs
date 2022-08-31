using ArrhythmicBattles.MainMenu;
using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Util;
using FlexFramework.Core;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainGame;

public class GameScene : Scene
{
    private readonly ABContext context;

    private PerspectiveCamera camera;
    private SkinnedModelEntity modelEntity;
    private Model model;

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
        
        Engine.Renderer.ClearColor = Color4.Black;
        alphaClipLayer = Engine.Renderer.GetLayerId(DefaultRenderer.AlphaClipLayerName);

        camera = new PerspectiveCamera();
        camera.Position = Vector3.UnitZ * 4.0f;

        model = new Model(@"Assets/Models/WalkAnim.dae");
        
        modelEntity = new SkinnedModelEntity();
        modelEntity.Model = model;
        modelEntity.Animation = model.Animations[0];
    }

    public override void Update(UpdateArgs args)
    {
        if (Engine.Input.GetKey(Keys.Escape))
        {
            Engine.LoadScene<MainMenuScene>(context);
        }
        
        modelEntity.Update(args);
        
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
        CameraData cameraData = camera.GetCameraData(Engine.ClientSize);
        
        MatrixStack.Push();
        // MatrixStack.Scale(0.0125f, 0.0125f, 0.0125f);
        modelEntity.Render(renderer, alphaClipLayer, MatrixStack, cameraData);
        MatrixStack.Pop();
    }

    public override void Dispose()
    {
        modelEntity.Dispose();
        model.Dispose();
        capture.Dispose();
    }
}