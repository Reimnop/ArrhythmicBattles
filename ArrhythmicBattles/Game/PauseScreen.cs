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

    private Node<ElementContainer> root;

    public PauseScreen(FlexFrameworkMain engine, ABScene scene)
    {
        this.engine = engine;
        this.scene = scene;
        
        inputProvider = scene.Context.InputSystem.AcquireInputProvider();

        background = new MeshEntity(DefaultAssets.QuadMesh);
        background.Color = new Color4(0.0f, 0.0f, 0.0f, 0.5f);
        
        root = BuildInterface();
        LayoutEngine.Layout(root, scene.ScreenBounds);
    }

    private Node<ElementContainer> BuildInterface()
    {
        var font = scene.Context.Font;
        var treeBuilder = new InterfaceTreeBuilder()
            .SetAnchor(Anchor.FillLeftEdge)
            .SetEdges(16.0f, 0.0f, 16.0f, -512.0f)
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new TextElement(font)
                {
                    Text = "Game paused!\nPress [Esc] to return to game.\n// or quit like a coward",
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(0.0f, -72.0f, 0.0f, 0.0f))
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new ABButtonElement(font, inputProvider, "BACK TO GAME")
                {
                    Click = () => scene.CloseScreen(this),
                    TextDefaultColor = Colors.AlternateTextColor
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 88.0f)))
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new ABButtonElement(font, inputProvider, "QUIT COWARDLY") // what a coward
                {
                    Click = () => engine.LoadScene(new MainMenuScene(scene.Context)),
                    TextDefaultColor = Colors.AlternateTextColor
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 152.0f)));

        return treeBuilder.Build();
    }
    
    public override void Update(UpdateArgs args)
    {
        background.Update(args);
        root.UpdateRecursively(args);

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
        root.RenderRecursively(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        inputProvider.Dispose();
        root.DisposeRecursively();
    }
}