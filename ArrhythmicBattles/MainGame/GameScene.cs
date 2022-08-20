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
    private readonly ABSfxContext sfxContext;

    public GameScene(ABContext context, ABSfxContext sfxContext)
    {
        this.context = context;
        this.sfxContext = sfxContext;
    }
    
    public override void Init()
    {
        Engine.Renderer.ClearColor = Color4.Black;
    }

    public override void Update(UpdateArgs args)
    {
        if (Engine.Input.GetKey(Keys.Escape))
        {
            Engine.LoadScene<MainMenuScene>(context, sfxContext);
        }
    }

    public override void Render(Renderer renderer)
    {
    }

    public override void Dispose()
    {
    }
}