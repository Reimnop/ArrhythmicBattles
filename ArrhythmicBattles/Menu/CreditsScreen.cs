using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Core;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Menu;

public class CreditsScreen : Screen, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly IInputProvider inputProvider;
    
    private readonly Element root;

    public CreditsScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider)
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
            new TextElement(engine, Constants.DefaultFontName)
            {
                Text = "Windows.\nWindows, what the fuck.\nWindows, your skin.\nWindows, your fucking skin.\n\n\"uwaaa <3\" - Windows 98, a VG moderator.\n\nLuce no\nLuce, your status.\nLuce.\n\nbtw Golden is the GOAT\n\nalso music made by LemmieDot",
                Width = Length.Full
            },
            new ABButtonElement(engine, inputProvider, "BACK")
            {
                Width = Length.Full,
                Height = 64.0f,
                Padding = 16.0f,
                TextDefaultColor = new Color4(233, 81, 83, 255),
                Click = () => scene.SwitchScreen(this, new SelectScreen(engine, scene, inputProvider))
            })
        {
            Width = Length.Full,
            Spacing = 16.0f
        };
    }

    public override void Update(UpdateArgs args)
    {
        root.UpdateRecursive(args);
        
        if (inputProvider.GetKeyDown(Keys.Escape))
        {
            scene.SwitchScreen(this, new SelectScreen(engine, scene, inputProvider));
        }
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