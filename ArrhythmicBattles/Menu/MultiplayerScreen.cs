using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

namespace ArrhythmicBattles.Menu;

public class MultiplayerScreen : Screen, IDisposable
{
    public override Vector2 Position { get; set; }

    private readonly FlexFrameworkMain engine;
    private readonly Element root;
    
    private readonly ABScene scene;
    private readonly IInputProvider inputProvider;

    public MultiplayerScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider)
    {
        this.engine = engine;
        this.scene = scene;
        this.inputProvider = inputProvider;
        
        Bounds bounds = new Bounds(48.0f, 306.0f, engine.Size.X, engine.Size.Y);
        
        root = InitUI();
        root.UpdateLayout(bounds);
    }

    private Element InitUI()
    {
        return new StackLayout(
            new ABButtonElement(engine, inputProvider, "Lorem ipsum dolor sit amet,")  
            {
                Width = Length.Full,
                Height = new Length(56.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            },
            new ABButtonElement(engine, inputProvider, "consectetur adipiscing elit.")
            {
                Width = Length.Full,
                Height = new Length(56.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            },
            new ABButtonElement(engine, inputProvider, "Nulla ut tincidunt quam.")
            {
                Width = Length.Full,
                Height = new Length(56.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            })
        {
            Width = new Length(512.0f, Unit.Pixel), 
            Height = new Length(200.0f, Unit.Pixel),
            Spacing = new Length(16.0f, Unit.Pixel)
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