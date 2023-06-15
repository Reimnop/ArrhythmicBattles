using ArrhythmicBattles.UserInterface;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;

namespace ArrhythmicBattles.Menu;

public class MainScreen : IScreen
{
    public Node<ElementContainer> RootNode { get; }
    
    public MainScreen(ABContext context, ScreenManager screenManager, ScopedInputProvider inputProvider)
    {
        // TODO: Implement interaction
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/PlayButton.json"))
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(16.0f, -80.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/SettingsButton.json"))
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(96.0f, -160.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/CreditsButton.json"))
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(176.0f, -240.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/ExitButton.json"))
                    .SetAnchor(Anchor.BottomLeft)
                    .SetEdges(-80.0f, 16.0f, 16.0f, -336.0f))
        );
    }

    public void Update(UpdateArgs args)
    {
        RootNode.UpdateRecursively(args);
    }

    public void Render(RenderArgs args)
    {
        RootNode.RenderRecursively(args);
    }
}