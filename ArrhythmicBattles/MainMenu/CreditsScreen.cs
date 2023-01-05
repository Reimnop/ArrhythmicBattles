using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainMenu;

public class CreditsScreen : Screen
{
    public override Vector2 Position { get; set; }

    private readonly FlexFrameworkMain engine;
    private readonly TextEntity textEntity;
    private readonly MainMenuScene mainMenuScene;
    private readonly InputInfo inputInfo;
    
    public CreditsScreen(InputInfo inputInfo, FlexFrameworkMain engine, MainMenuScene mainMenuScene)
    {
        this.engine = engine;
        this.mainMenuScene = mainMenuScene;
        this.inputInfo = inputInfo;

        textEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-regular"));
        textEntity.BaselineOffset = 24.0f;
        textEntity.Text = "Windows.\nWindows, what the fuck.\nWindows, your skin.\nWindows, your fucking skin.\nLuce no\nLuce of muck";
    }
    
    public override void Update(UpdateArgs args)
    {
        textEntity.Update(args);

        if (mainMenuScene.Context.InputSystem.GetKeyDown(inputInfo.InputCapture, Keys.Escape))
        {
            mainMenuScene.SwitchScreen(new SelectScreen(inputInfo, engine, mainMenuScene));
        }
    }
    
    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0f);
        textEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }

    public override void Dispose()
    {
        textEntity.Dispose();
    }
}