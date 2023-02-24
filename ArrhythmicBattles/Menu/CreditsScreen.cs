using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.Entities;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Menu;

public class CreditsScreen : Screen, IDisposable
{
    public override Vector2 Position { get; set; }

    private readonly FlexFrameworkMain engine;
    private readonly TextEntity textEntity;
    private readonly ABScene scene;
    private readonly IInputProvider inputProvider;
    
    public CreditsScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider)
    {
        this.engine = engine;
        this.scene = scene;
        this.inputProvider = inputProvider;

        textEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-regular"));
        textEntity.BaselineOffset = 24;
        textEntity.Text = "Windows.\nWindows, what the fuck.\nWindows, your skin.\nWindows, your fucking skin.\nLuce no\nLuce.\n\nPress [Esc] to return";
    }
    
    public override void Update(UpdateArgs args)
    {
        textEntity.Update(args);

        if (inputProvider.GetKeyDown(Keys.Escape))
        {
            scene.SwitchScreen(this, new SelectScreen(engine, scene, inputProvider));
        }
    }
    
    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        textEntity.Dispose();
    }
}