using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainMenu;

public class CreditsScreen : Screen
{
    public override Vector2d Position { get; set; }

    private readonly FlexFrameworkMain engine;
    private readonly TextEntity textEntity;
    private readonly MainMenuScene mainMenuScene;
    private readonly InputCapture capture;

    public CreditsScreen(FlexFrameworkMain engine, MainMenuScene mainMenuScene)
    {
        this.engine = engine;
        this.mainMenuScene = mainMenuScene;

        capture = mainMenuScene.Context.InputSystem.AcquireCapture();

        textEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-regular"));
        textEntity.BaselineOffset = 24.0;
        textEntity.Text = "Windows.\nWindows, what the fuck.\nWindows, your skin.\nWindows, your fucking skin.\nLuce no\n\nPress Esc to return to main menu";
    }
    
    public override void Update(UpdateArgs args)
    {
        textEntity.Update(args);

        if (mainMenuScene.Context.InputSystem.GetKeyDown(capture, Keys.Escape))
        {
            mainMenuScene.SwitchScreen<SelectScreen>(engine, mainMenuScene);
        }
    }
    
    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0);
        textEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }

    public override void Dispose()
    {
        textEntity.Dispose();
        capture.Dispose();
    }
}