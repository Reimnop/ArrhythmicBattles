using ArrhythmicBattles.MainMenu;
using ArrhythmicBattles.Util;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Settings;

public class SettingsScene : GuiScene
{
    private TextEntity textEntity;

    private readonly ABContext context;
    private readonly ABSfxContext sfxContext;

    public SettingsScene(ABContext context, ABSfxContext sfxContext)
    {
        this.context = context;
        this.sfxContext = sfxContext;
    }
    
    public override void Init()
    {
        base.Init();
        
        textEntity = new TextEntity(Engine, Engine.TextResources.GetFont("inconsolata-regular"));
        textEntity.BaselineOffset = 24.0;
        textEntity.Text = "Luce, do not.\nLuce, your status.\nWindows.\nWindows, what the fuck.\nWINDOWS.\n\nPress Esc to return to main menu";
    }

    public override void Update(UpdateArgs args)
    {
        textEntity.Update(args);
        
        if (Engine.Input.GetKeyDown(Keys.Escape))
        {
            Engine.LoadScene<MainMenuScene>(context, sfxContext);
        }
    }

    public override void Render(Renderer renderer)
    {
        CameraData cameraData = Camera.GetCameraData(Engine.ClientSize);
        textEntity.Render(renderer, GuiLayerId, MatrixStack, cameraData);
    }

    public override void Dispose()
    {
        textEntity.Dispose();
    }
}