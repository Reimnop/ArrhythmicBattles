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

    private readonly FlexFrameworkMain engine;
    private readonly ScopedInputProvider inputProvider;

    public PauseScreen(FlexFrameworkMain engine, ScreenManager screenManager, ABContext context)
    {
        this.engine = engine;

        inputProvider = context.InputSystem.AcquireInputProvider();

        background = new MeshEntity(DefaultAssets.QuadMesh);
        background.Color = new Color4(0.0f, 0.0f, 0.0f, 0.5f);
        
        var resourceManager = context.ResourceManager;
        var font = resourceManager.Get<Font>(Constants.RegularFontPath);
        
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.FillLeftEdge)
                .SetEdges(16.0f, 0.0f, 16.0f, -512.0f)
                .SetElement(new TextElement(font)
                {
                    Text = "Woops! It looks like I haven't implemented this yet.\nToo bad!\n\nOh and, you can't even unpause now. Tough luck."
                })
        );
    }

    public void Update(UpdateArgs args)
    {
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