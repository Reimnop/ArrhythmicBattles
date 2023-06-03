using ArrhythmicBattles.UserInterface;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Util;

namespace ArrhythmicBattles.Menu;

public class CreditsScreen : Screen, IDisposable
{
    public override Node<ElementContainer> RootNode { get; }

    public CreditsScreen(FlexFrameworkMain engine, ScreenManager screenManager, ABContext context, ScopedInputProvider inputProvider)
    {
        var font = context.Font;
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.FillTopEdge)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new TextElement(font)
                    {
                        Text = "Windows.\nWindows, what the fuck.\nWindows, your fucking skin.\n\n\"uwaaa <3\" - Windows 98, a VG moderator.\n\nLuce of muck\nLuce, your status.\n\nmusic made by LemmieDot btw"
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(0.0f, -TextHelper.CalculateTextHeight(font, 10), 0.0f, 0.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABButtonElement(font, inputProvider, "BACK")
                    {
                        Click = () => screenManager.Switch(this, new SelectScreen(engine, screenManager, context, inputProvider)),
                        TextDefaultColor = Colors.TextAlternate
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, TextHelper.CalculateTextHeight(font, 10) + 16.0f)))
        );
    }

    public override void Update(UpdateArgs args)
    {
        RootNode.UpdateRecursively(args);
    }

    public override void Render(RenderArgs args)
    {
        RootNode.RenderRecursively(args);
    }

    public void Dispose()
    {
        RootNode.DisposeRecursively();
    }
}