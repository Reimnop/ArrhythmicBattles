using ArrhythmicBattles.Game;
using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;

namespace ArrhythmicBattles.Menu;

public class PlayScreen : IScreen
{
    public Node<ElementContainer> RootNode { get; }
    
    public PlayScreen(ABContext context, ScreenManager screenManager, ScopedInputProvider inputProvider)
    {
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/PlayButton.json")
                    {
                        Click = () => context.Engine.LoadScene(new GameScene(context))
                    })
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(16.0f, -80.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/BackButton.json") 
                    {
                        Click = () => screenManager.Switch(this, new MainScreen(context, screenManager, inputProvider))
                    })
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