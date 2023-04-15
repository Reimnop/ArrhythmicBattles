﻿using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Core;

public abstract class ABScene : Scene, IDisposable
{
    public Bounds ScreenBounds { get; protected set; }

    public ABContext Context { get; }
    
    protected MatrixStack MatrixStack { get; } = new MatrixStack();
    protected GuiCamera GuiCamera { get; private set; }
    protected int GuiLayerId { get; private set; }

    protected LayeredScreenHandler ScreenHandler { get; } = new LayeredScreenHandler();

    public ABScene(ABContext context)
    {
        Context = context;
    }

    public override void Init()
    {
        Renderer renderer = Engine.Renderer;

        renderer.ClearColor = new Color4(33, 33, 33, 255);
        GuiLayerId = renderer.GetLayerId("gui");

        GuiCamera = new GuiCamera(Engine);
    }

    public override void Update(UpdateArgs args)
    {
        ScreenHandler.Update(args);
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

    public virtual void Dispose()
    {
        ScreenHandler.Dispose();
    }
}