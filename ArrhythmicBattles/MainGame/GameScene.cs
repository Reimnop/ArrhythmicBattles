using ArrhythmicBattles.MainMenu;
using FlexFramework.Core;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainGame;

public class GameScene : Scene
{
    private readonly ABContext context;

    public GameScene(ABContext context)
    {
        this.context = context;
    }
    
    public override void Init()
    {
        Engine.Renderer.ClearColor = Color4.Black;
    }

    public override void Update(UpdateArgs args)
    {
        if (Engine.Input.GetKey(Keys.Escape))
        {
            Engine.LoadScene<MainMenuScene>(context);
        }
    }

    public override void Render(Renderer renderer)
    {
    }

    public override void Dispose()
    {
    }
}