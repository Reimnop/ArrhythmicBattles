using ArrhythmicBattles.UI;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Util;

public abstract class ABScene : Scene
{
    public ABContext Context { get; }

    protected GuiCamera Camera { get; private set; }
    protected int GuiLayerId { get; private set; }

    public ABScene(ABContext context)
    {
        Context = context;
    }

    public override void Init()
    {
        Renderer renderer = Engine.Renderer;

        renderer.ClearColor = new Color4(33, 33, 33, 255);
        GuiLayerId = renderer.GetLayerId("gui");

        Camera = new GuiCamera(Engine);
    }
    
    public abstract void SetScreen(Screen? screen);
}