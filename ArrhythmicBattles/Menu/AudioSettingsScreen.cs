using ArrhythmicBattles.UserInterface;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;

namespace ArrhythmicBattles.Menu;

public class AudioSettingsScreen : Screen, IDisposable
{
    public override Node<ElementContainer> RootNode { get; }

    public AudioSettingsScreen(FlexFrameworkMain engine, ScreenManager screenManager, ABContext context, ScopedInputProvider inputProvider)
    {
        var settings = context.Settings;
        var font = context.Font;

        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABSliderElement(font, inputProvider, "SFX VOLUME")
                    {
                        Value = settings.SfxVolume,
                        ValueChanged = value => settings.SfxVolume = value
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(0.0f, -64.0f, 0.0f, 0.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABSliderElement(font, inputProvider, "MUSIC VOLUME")
                    {
                        Value = settings.MusicVolume,
                        ValueChanged = value => settings.MusicVolume = value
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 64.0f)))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABButtonElement(font, inputProvider, "BACK")
                    {
                        Click = () =>
                            screenManager.Switch(this, new SettingsScreen(engine, screenManager, context, inputProvider)),
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