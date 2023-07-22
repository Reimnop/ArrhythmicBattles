using ArrhythmicBattles.Menu;
using ArrhythmicBattles.UserInterface;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Text;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game;

public class PauseScreen : IScreen, IDisposable
{
    public Node<ElementContainer> RootNode { get; }

    private readonly MeshEntity background;

    private readonly FlexFrameworkApplication engine;
    private readonly ABContext context;
    private readonly ScopedInputProvider inputProvider;
    
    private readonly TextElement textElement;
    private float t;

    public PauseScreen(FlexFrameworkApplication engine, ScreenManager screenManager, ABContext context)
    {
        this.engine = engine;
        this.context = context;

        inputProvider = context.InputSystem.AcquireInputProvider();

        background = new MeshEntity(DefaultAssets.QuadMesh);
        background.Color = new Color4(0.0f, 0.0f, 0.0f, 0.5f);
        
        var resourceManager = context.ResourceManager;
        var font = resourceManager.Get<Font>(Constants.RegularFontPath);
        
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.FillLeftEdge)
                .SetEdges(16.0f, 0.0f, 16.0f, -512.0f)
                .SetElement(textElement = new TextElement(font)
                {
                    Text = "Woops! It looks like I haven't implemented this yet."
                })
        );
    }

    public void Update(UpdateArgs args)
    {
        t += args.DeltaTime;
        textElement.Text = $"Woops! It looks like I haven't implemented this yet.\n\nReturning to menu in {(int)(3.0f - t) + 1}";
        
        // Return to menu after 3 seconds
        if (t >= 3.0f)
        {
            engine.SceneManager.LoadScene(() => new MainMenuScene(context));
        }
        
        RootNode.UpdateRecursively(args);
    }
    
    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
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