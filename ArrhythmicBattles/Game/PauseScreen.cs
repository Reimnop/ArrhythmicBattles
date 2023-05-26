using ArrhythmicBattles.Core;
using ArrhythmicBattles.Menu;
using ArrhythmicBattles.UserInterface;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Game;

public class PauseScreen : Screen, IDisposable
{
    private readonly MeshEntity background;

    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly ScopedInputProvider inputProvider;

    private Element root;

    public PauseScreen(FlexFrameworkMain engine, ABScene scene)
    {
        this.engine = engine;
        this.scene = scene;
        
        inputProvider = scene.Context.InputSystem.AcquireInputProvider();

        background = new MeshEntity(DefaultAssets.QuadMesh);
        background.Color = new Color4(0.0f, 0.0f, 0.0f, 0.5f);
        
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
                Text = "Game paused!\nPress [Esc] to return to game.",
                Width = Length.Full
            },
            new StackLayoutElement(
                Direction.Horizontal,
                new ABButtonElement(font, inputProvider, "BACK")
                {
                    TextDefaultColor = new Color4(233, 81, 83, 255),
                    Width = new Length(0.5f, Unit.Percent),
                    Height = 64.0f,
                    Padding = 16.0f,
                    Click = () => scene.CloseScreen(this)
                },
                new ABButtonElement(font, inputProvider, "QUIT COWARDLY") // insults the player
                {
                    TextDefaultColor = new Color4(233, 81, 83, 255),
                    Width = new Length(0.5f, Unit.Percent),
                    Height = 64.0f,
                    Padding = 16.0f,
                    Click = () => engine.LoadScene(new MainMenuScene(scene.Context))
                })
            {
                Width = 384.0f
            })
        {
            Width = Length.Full,
            Spacing = 16.0f
        };
    }
    
    public override void Update(UpdateArgs args)
    {
        background.Update(args);
        root.UpdateRecursive(args);

        if (inputProvider.GetKeyDown(Keys.Escape))
        {
            scene.CloseScreen(this);
        }
    }
    
    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(0.5f, 0.5f, 0.0f);
        matrixStack.Scale(engine.ClientSize.X, engine.ClientSize.Y, 1.0f);
        background.Render(args);
        matrixStack.Pop();
        
        matrixStack.Push();
        root.RenderRecursive(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        inputProvider.Dispose();
        root.DisposeRecursive();
    }
}