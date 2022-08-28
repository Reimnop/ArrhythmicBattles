using ArrhythmicBattles.MainMenu;
using ArrhythmicBattles.Modelling;
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
    private ModelEntity modelEntity;
    private Model model;

    private int opaqueLayer;

    private float yRotation = 0.0f;
    private float xRotation = 0.0f;

    public GameScene(ABContext context)
    {
        this.context = context;
    }
    
    public override void Init()
    {
        Engine.Renderer.ClearColor = Color4.Black;
        opaqueLayer = Engine.Renderer.GetLayerId("opaque");

        camera = new PerspectiveCamera();
        camera.Position = Vector3.UnitZ * 4.0f;

        model = new Model("testanim.dae");
        
        modelEntity = new ModelEntity();
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
        
        // never, ever input like this
        Input input = Engine.Input;
        
        Vector3 forward = Vector3.Transform(-Vector3.UnitZ, camera.Rotation);
        Vector3 right = Vector3.Transform(Vector3.UnitX, camera.Rotation);

        Vector2 movement = input.GetMovement();

        Vector3 move = forward * movement.Y + right * movement.X;
        camera.Position += move * 8.0f * args.DeltaTime;

        if (input.GetMouse(MouseButton.Right))
        {
            Vector2 delta = input.MouseDelta / 480.0f;

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
        modelEntity.Render(renderer, opaqueLayer, MatrixStack, cameraData);
        MatrixStack.Pop();
    }

    public override void Dispose()
    {
        modelEntity.Dispose();
        model.Dispose();
    }
}