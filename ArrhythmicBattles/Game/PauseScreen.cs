using ArrhythmicBattles.Core;
using ArrhythmicBattles.UserInterface;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Game;

public class PauseScreen : Screen, IDisposable
{
    private readonly TextEntity textEntity;
    private readonly MeshEntity background;

    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly ScopedInputProvider inputProvider;

    public PauseScreen(FlexFrameworkMain engine, ABScene scene)
    {
        this.engine = engine;
        this.scene = scene;
        
        inputProvider = scene.Context.InputSystem.AcquireInputProvider();
        
        EngineAssets assets = engine.DefaultAssets;

        background = new MeshEntity();
        background.Mesh = engine.ResourceRegistry.GetResource(assets.QuadMesh);
        background.Color = new Color4(0.0f, 0.0f, 0.0f, 0.5f);

        textEntity = new TextEntity(scene.Context.Font);
        textEntity.BaselineOffset = scene.Context.Font.Metrics.Height;
        textEntity.Text = "Game paused!\n\nPress [Esc] to return";
    }
    
    public override void Update(UpdateArgs args)
    {
        background.Update(args);
        textEntity.Update(args);

        if (inputProvider.GetKeyDown(Keys.Escape))
        {
            scene.CloseScreen(this);
        }
    }
    
    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(0.5f, 0.5f, 0.0f);
        matrixStack.Scale(engine.ClientSize.X, engine.ClientSize.Y, 1.0f);
        background.Render(args);
        matrixStack.Pop();
        
        matrixStack.Push();
        textEntity.Render(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        inputProvider.Dispose();
    }
}