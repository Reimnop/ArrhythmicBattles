﻿using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Core;

public abstract class ABScene : Scene, IDisposable
{
    public Bounds ScreenBounds => GetScreenBounds();

    public ABContext Context { get; }
    protected EntityManager EntityManager { get; } = new();
    protected MatrixStack MatrixStack { get; } = new();
    protected GuiCamera GuiCamera { get; }

    protected LayeredScreenHandler ScreenHandler { get; } = new();
    
    // Rendering stuff
    private CommandList commandList = new();

    public ABScene(ABContext context) : base(context.Engine)
    {
        Context = context;

        var renderer = Engine.Renderer;
        renderer.ClearColor = new Color4(33, 33, 33, 255);
        GuiCamera = new GuiCamera(Engine);
    }

    public override void Update(UpdateArgs args)
    {
        EntityManager.Update(args);
        ScreenHandler.Update(args);
    }
    
    protected virtual Bounds GetScreenBounds()
    {
        return new Bounds(0.0f, 0.0f, Engine.ClientSize.X, Engine.ClientSize.Y);
    }

    public virtual void OpenScreen(Screen screen)
    {
        ScreenHandler.OpenScreen(screen);
    }

    public virtual void CloseScreen(Screen screen)
    {
        ScreenHandler.CloseScreen(screen);
    }

    public virtual void SwitchScreen(Screen before, Screen after)
    {
        ScreenHandler.SwitchScreen(before, after);
    }

    public override void Render(Renderer renderer)
    {
        // Clear command list
        commandList.Clear();
        
        // Queue render commands
        RenderScene(commandList);
        
        // Render and present
        renderer.Render(Engine.ClientSize, commandList, Context.RenderBuffer);
        Engine.Present(Context.RenderBuffer);
    }
    
    protected abstract void RenderScene(CommandList commandList);

    public virtual void Dispose()
    {
        EntityManager.Dispose();
        ScreenHandler.Dispose();
    }
}