using ArrhythmicBattles.UserInterface;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Util;

namespace ArrhythmicBattles.Menu;

public class EmotionalDamageScreen : Screen, IDisposable
{
    public override Node<ElementContainer> RootNode { get; }

    public EmotionalDamageScreen(FlexFrameworkMain engine, ScreenManager screenManager, ABContext context, ScopedInputProvider inputProvider)
    {
        var font = context.Font;

        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.FillTopEdge)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new TextElement(font)
                    {
                        Text = "I can't believe you fell for that...\nDid you really think a multiplayer demo existed??"
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(0.0f, -TextHelper.CalculateTextHeight(font, 2), 0.0f, 0.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABButtonElement(font, inputProvider, "BACK")
                    {
                        Click = () => screenManager.Switch(this, new SelectScreen(engine, screenManager, context, inputProvider)),
                        TextDefaultColor = Colors.TextAlternate
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, TextHelper.CalculateTextHeight(font, 2) + 16.0f)))
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