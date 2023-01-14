using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainGame;

public class PauseScreen : Screen
{
    public override Vector2 Position { get; set; }
    
    private readonly TextEntity textEntity;
    private readonly MeshEntity background;

    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly InputSystem inputSystem;
    private readonly InputInfo inputInfo;

    public PauseScreen(FlexFrameworkMain engine, ABScene scene)
    {
        this.engine = engine;
        this.scene = scene;
        
        inputSystem = scene.Context.InputSystem;
        inputInfo = inputSystem.GetInputInfo();

        background = new MeshEntity();
        background.Mesh = engine.PersistentResources.QuadMesh;
        background.Color = new Color4(0.0f, 0.0f, 0.0f, 0.5f);

        textEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-regular"));
        textEntity.BaselineOffset = 24.0f;
        textEntity.Text = "Game paused!\n\nPress [Esc] to return";
    }
    
    public override void Update(UpdateArgs args)
    {
        background.Update(args);
        textEntity.Update(args);

        if (inputSystem.GetKeyDown(inputInfo.InputCapture, Keys.Escape))
        {
            scene.CloseScreen(this);
        }
    }
    
    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(0.5f, 0.5f, 0.0f);
        matrixStack.Scale(engine.ClientSize.X, engine.ClientSize.Y, 1.0f);
        background.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
        
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0f);
        textEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }

    public override void Dispose()
    {
        textEntity.Dispose();
        inputInfo.Dispose();
    }
}