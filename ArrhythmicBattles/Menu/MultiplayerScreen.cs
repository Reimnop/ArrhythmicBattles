using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Core;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Menu;

public class MultiplayerScreen : Screen, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly IInputProvider inputProvider;
    
    private readonly Element root;

    public MultiplayerScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider)
    {
        this.engine = engine;
        this.scene = scene;
        this.inputProvider = inputProvider;

        root = BuildInterface();
        root.UpdateLayout(scene.ScreenBounds);
    }

    private Element BuildInterface()
    {
        var font = scene.Context.Font;
        
        return new StackLayoutElement(
            Direction.Vertical,
            new TextElement(font)
            {
                Text = "Feature not yet available!\nJust pretend this is a multiplayer menu.",
                Width = Length.Full
            },
            new ABButtonElement(font, inputProvider, "BACK")
            {
                TextDefaultColor = new Color4(233, 81, 83, 255),
                Width = Length.Full,
                Height = 64.0f,
                Padding = 16.0f,
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