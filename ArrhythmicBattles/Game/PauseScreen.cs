using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

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
        
        EngineResources resources = engine.Resources;

        background = new MeshEntity();
        background.Mesh = engine.ResourceManager.GetResource<Mesh<Vertex>>(resources.QuadMesh);
        background.Color = new Color4(0.0f, 0.0f, 0.0f, 0.5f);

        Font font = engine.TextResources.GetFont("inconsolata-regular");
        
        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
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
        textEntity.Dispose();
        inputProvider.Dispose();
    }
}