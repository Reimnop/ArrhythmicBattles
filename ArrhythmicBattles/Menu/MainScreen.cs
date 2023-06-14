using ArrhythmicBattles.UserInterface;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Util;

namespace ArrhythmicBattles.Menu;

public class MainScreen : IScreen
{
    public Node<ElementContainer> RootNode { get; }
    
    public MainScreen(FlexFrameworkMain engine, ScreenManager screenManager, ABContext context, ScopedInputProvider inputProvider)
    {
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .SetElement(new TextElement(context.Font)
                {
                    Text = "Arrhythmic Battles"
                })
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