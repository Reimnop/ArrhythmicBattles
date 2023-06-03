using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Core;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Menu;

public class SettingsScreen : Screen, IDisposable
{
    public override Node<ElementContainer> RootNode { get; }

    public SettingsScreen(FlexFrameworkMain engine, ScreenManager screenManager, ABContext context, ScopedInputProvider inputProvider)
    {
        var font = context.Font;
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.FillTopEdge)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABButtonElement(font, inputProvider, "VIDEO")
                    {
                        // TODO: Add video settings screen
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(0.0f, -64.0f, 0.0f, 0.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABButtonElement(font, inputProvider, "AUDIO")
                    {
                        Click = () => screenManager.Switch(this, new AudioSettingsScreen(engine, screenManager, context, inputProvider))
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 64.0f)))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABButtonElement(font, inputProvider, "BACK")
                    {
                        Click = () => screenManager.Switch(this, new SelectScreen(engine, screenManager, context, inputProvider)),
                        TextDefaultColor = Colors.TextAlternate
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 128.0f)))
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