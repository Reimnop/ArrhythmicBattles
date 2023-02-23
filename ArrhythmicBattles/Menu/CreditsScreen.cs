using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.Entities;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Menu;

public class CreditsScreen : Screen, IDisposable
{
    public override Vector2 Position { get; set; }

    private readonly FlexFrameworkMain engine;
    private readonly TextEntity textEntity;
    private readonly ABScene scene;
    private readonly InputInfo inputInfo;
    
    public CreditsScreen(FlexFrameworkMain engine, ABScene scene, InputInfo inputInfo)
    {
        this.engine = engine;
        this.scene = scene;
        this.inputInfo = inputInfo;

        textEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-regular"));
        textEntity.BaselineOffset = 24.0f;
        textEntity.Text = "Windows.\nWindows, what the fuck.\nWindows, your skin.\nWindows, your fucking skin.\nLuce no\nLuce.\n\nPress [Esc] to return";
    }
    
    public override void Update(UpdateArgs args)
    {
        textEntity.Update(args);

        if (scene.Context.InputSystem.GetKeyDown(inputInfo.InputCapture, Keys.Escape))
        {
            scene.SwitchScreen(this, new SelectScreen(engine, scene, inputInfo));
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