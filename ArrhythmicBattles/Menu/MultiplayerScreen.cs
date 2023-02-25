using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
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
        return new StackLayoutElement(
            new ABButtonElement(engine, inputProvider, "Lorem ipsum dolor sit amet,")  
            {
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            },
            new ABButtonElement(engine, inputProvider, "consectetur adipiscing elit.")
            {
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            },
            new ABButtonElement(engine, inputProvider, "Nulla ut tincidunt quam.")
            {
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            })
        {
            Width = Length.Full
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