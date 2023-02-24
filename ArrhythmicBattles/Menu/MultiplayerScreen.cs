using ArrhythmicBattles.UI;
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
        Font font = engine.TextResources.GetFont("inconsolata-regular");

        return new StackLayout(
            new ButtonElement(
                inputProvider,
                new TextElement(engine, font)
                {
                    Text = "Lorem ipsum dolor sit amet,",
                    Color = Color4.Black,
                    Width = Length.Full,
                    Height = Length.Full
                })
            {
                Width = Length.Full,
                Height = new Length(56.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            },
            new ButtonElement(
                inputProvider,
                new TextElement(engine, font)
                {
                    Text = "consectetur adipiscing elit.",
                    Color = Color4.Black,
                    Width = Length.Full,
                    Height = Length.Full
                })
            {
                Width = Length.Full,
                Height = new Length(56.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            },
            new ButtonElement(
                inputProvider,
                new TextElement(engine, font)
                {
                    Text = "Nulla ut tincidunt quam.",
                    Color = Color4.Black,
                    Width = Length.Full,
                    Height = Length.Full
                })
            {
                Width = Length.Full,
                Height = new Length(56.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            })
        {
            Width = new Length(512.0f, Unit.Pixel), 
            Height = new Length(240.0f, Unit.Pixel),
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