using ArrhythmicBattles.Core;
using ArrhythmicBattles.Menu;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Editor;

public class EditorScene : ABScene
{
    public EditorScene(ABContext context) : base(context)
    {
    }

    public override void Update(UpdateArgs args)
    {
        if (Engine.Input.GetKeyDown(Keys.Escape))
        {
            Engine.SceneManager.LoadScene(() => new MainMenuScene(Context));
        }
    }

    protected override void RenderScene(CommandList commandList)
    {
        
    }
}