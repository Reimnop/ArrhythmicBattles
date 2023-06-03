using System.Diagnostics;
using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Core;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Text;
using FlexFramework.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Menu;

public class MultiplayerScreen : Screen, IDisposable
{
    public override Node<ElementContainer> RootNode { get; }

    public MultiplayerScreen(FlexFrameworkMain engine, ScreenManager screenManager, ABContext context, ScopedInputProvider inputProvider)
    {
        var font = context.Font;
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.FillTopEdge)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new TextElement(font)
                    {
                        Text = "Multiplayer isn't available yet!\nClick the button below to go back to the main menu.",
                        VerticalAlignment = VerticalAlignment.Top
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
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ABButtonElement(font, inputProvider, "TRY DEMO")
                    {
                        Click = () =>
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "https://www.youtube.com/watch?v=dQw4w9WgXcQ", // hehe
                                UseShellExecute = true
                            });

                            screenManager.Switch(this, new EmotionalDamageScreen(engine, screenManager, context, inputProvider));
                        },
                        TextDefaultColor = Colors.TextAlternate
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, TextHelper.CalculateTextHeight(font, 2) + 80.0f)))
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