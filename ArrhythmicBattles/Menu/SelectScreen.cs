using ArrhythmicBattles.Game;
using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Menu;

public class SelectScreen : Screen, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly IInputProvider inputProvider;
    
    private readonly Element root;

    public SelectScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider)
    {
        this.engine = engine;
        this.scene = scene;
        this.inputProvider = inputProvider;

        root = BuildInterface();
        root.UpdateLayout(scene.ScreenBounds);
    }

    private Element BuildInterface()
    {
        return new StackLayoutElement(
            Direction.Vertical,
            new ABButtonElement(engine, inputProvider, "SINGLEPLAYER")  
            {
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel),
                Click = () => engine.LoadScene(new GameScene(scene.Context))
            },
            new ABButtonElement(engine, inputProvider, "MULTIPLAYER")
            {
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel),
                Click = () => scene.SwitchScreen(this, new MultiplayerScreen(engine, scene, inputProvider))
            },
            new ABButtonElement(engine, inputProvider, "SETTINGS")
            {
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel),
                Click = () => scene.SwitchScreen(this, new SettingsScreen(engine, scene, inputProvider))
            },
            new ABButtonElement(engine, inputProvider, "CREDITS")
            {
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel),
                Click = () => scene.SwitchScreen(this, new CreditsScreen(engine, scene, inputProvider))
            },
            new ABButtonElement(engine, inputProvider, "EXIT")
            {
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel),
                TextDefaultColor = new Color4(233, 81, 83, 255),
                Click = () => scene.CloseScreen(this)
            })
        {
            Width = Length.Full
        };
    }

    public override void Update(UpdateArgs args)
    {
        root.UpdateRecursive(args);
    }

    public override void Render(RenderArgs args)
    {
        root.RenderRecursive(args);
    }

    public void Dispose()
    {
        root.DisposeRecursive();
    }
}