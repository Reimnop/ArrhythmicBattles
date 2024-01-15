using ArrhythmicBattles.Editor;
using ArrhythmicBattles.Game;
using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Text;
using FlexFramework.Util;

namespace ArrhythmicBattles.Menu;

public class CreditsScreen : IScreen
{
    public Node<ElementContainer> RootNode { get; }
    
    public CreditsScreen(ABContext context, ScreenManager screenManager, ScopedInputProvider inputProvider)
    {
        var resourceManager = context.ResourceManager;
        var regularFont = resourceManager.Get<Font>(Constants.RegularFontPath);
        var boldFont = resourceManager.Get<Font>(Constants.BoldFontPath);
        
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new TextElement(boldFont)
                    {
                        Text = "CREDITS",
                        EmSize = 1.5f
                    })
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(16.0f, -64.0f, 32.0f, -512.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new TextElement(regularFont)
                    {
                        Text = "Programming - Reimnop\n" +
                               "Management  - MekaniQ\n\n" +
                               "\"uwaaa <3\" - Windows 98, a Vitamin Games moderator."
                    })
                    .SetAnchor(Anchor.Fill)
                    .SetEdges(64.0f, 16.0f, 16.0f, 16.0f))
                .AddChild(new InterfaceTreeBuilder() // Back button
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