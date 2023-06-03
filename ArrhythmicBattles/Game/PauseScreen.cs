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
    public override Node<ElementContainer> RootNode { get; }

    private readonly MeshEntity background;

    private readonly FlexFrameworkMain engine;
    private readonly ScopedInputProvider inputProvider;

    public PauseScreen(FlexFrameworkMain engine, ScreenManager screenManager, ABContext context)
    {
        this.engine = engine;

        inputProvider = context.InputSystem.AcquireInputProvider();

        background = new MeshEntity(DefaultAssets.QuadMesh);
        background.Color = new Color4(0.0f, 0.0f, 0.0f, 0.5f);
        
        var font = context.Font;
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.FillLeftEdge)
                .SetEdges(16.0f, 0.0f, 16.0f, -512.0f)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new TextElement(font)
                    {
                        Text = "Game paused!\nPress [Esc] to return to game.\n// or quit like a coward",
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(0.0f, -TextHelper.CalculateTextHeight(font, 3), 0.0f, 0.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABButtonElement(font, inputProvider, "BACK TO GAME")
                    {
                        Click = () => screenManager.Close(this),
                        TextDefaultColor = Colors.TextAlternate
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, TextHelper.CalculateTextHeight(font, 3) + 16.0f)))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABButtonElement(font, inputProvider, "QUIT COWARDLY") // what a coward
                    {
                        Click = () => engine.LoadScene(new MainMenuScene(context)),
                        TextDefaultColor = Colors.TextAlternate
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, TextHelper.CalculateTextHeight(font, 3) + 80.0f)))
        );
    }

    public override void Update(UpdateArgs args)
    {
        background.Update(args);
        RootNode.UpdateRecursively(args);
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
        RootNode.RenderRecursively(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        inputProvider.Dispose();
        RootNode.DisposeRecursively();
    }
}