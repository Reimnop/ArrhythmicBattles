using ArrhythmicBattles.Editor;
using ArrhythmicBattles.Game;
using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;

namespace ArrhythmicBattles.Menu;

public class MainScreen : IScreen
{
    public Node<ElementContainer> RootNode { get; }
    
    public MainScreen(ABContext context, ScreenManager screenManager, ScopedInputProvider inputProvider)
    {
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/PlayButton.json")
                    {
                        Click = () => screenManager.Switch(this, new PlayScreen(context, screenManager, inputProvider))
                    })
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(16.0f, -80.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/EditorButton.json")
                    {
                        Click = () => context.Engine.SceneManager.LoadScene(() => new EditorScene(context))
                    })
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(96.0f, -160.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/SettingsButton.json"))
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(176.0f, -240.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/CreditsButton.json")
                    {
                        Click = () => screenManager.Switch(this, new CreditsScreen(context, screenManager, inputProvider))
                    })
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(256.0f, -320.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/ExitButton.json") 
                    {
                        Click = () => screenManager.Close(this)
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