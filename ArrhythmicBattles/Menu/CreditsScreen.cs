using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.Entities;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

namespace ArrhythmicBattles.Menu;

public class CreditsScreen : Screen, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly TextEntity textEntity;
    private readonly ABScene scene;
    private readonly IInputProvider inputProvider;
    
    public CreditsScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider)
    {
        this.engine = engine;
        this.scene = scene;
        this.inputProvider = inputProvider;

        Font font = engine.TextResources.GetFont("inconsolata-regular");
        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
        textEntity.Text = "Windows.\nWindows, what the fuck.\nWindows, your skin.\nWindows, your fucking skin.\n\n\"uwaaa <3\" - Windows 98, a VG moderator.\n\nLuce no\nLuce.\n\nalso music made by LemmieDot lmao\n\nPress [Esc] to return";
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
        matrixStack.Translate(48.0f, 306.0f, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        textEntity.Dispose();
    }
}