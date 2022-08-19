using FlexFramework.Core;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Util;

public abstract class GuiScene : Scene
{
    protected GuiCamera Camera { get; private set; }
    protected int GuiLayerId { get; private set; }

    public override void Init()
    {
        Renderer renderer = Engine.Renderer;

        renderer.ClearColor = new Color4(33, 33, 33, 255);
        GuiLayerId = renderer.GetLayerId("gui");

        Camera = new GuiCamera(Engine);
    }
}