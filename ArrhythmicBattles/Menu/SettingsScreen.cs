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
    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly ScopedInputProvider inputProvider;
    
    private readonly Node<ElementContainer> root;

    public SettingsScreen(FlexFrameworkMain engine, ABScene scene, ScopedInputProvider inputProvider)
    {
        this.engine = engine;
        this.scene = scene;
        this.inputProvider = inputProvider;

        root = BuildInterface();
        LayoutEngine.Layout(root, scene.ScreenBounds);
    }

    private Node<ElementContainer> BuildInterface()
    {
        var font = scene.Context.Font;
        var treeBuilder = new InterfaceTreeBuilder()
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
                    Click = () => scene.SwitchScreen(this, new AudioSettingsScreen(engine, scene, inputProvider))
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 64.0f)))
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new ABButtonElement(font, inputProvider, "BACK")
                {
                    Click = () => scene.SwitchScreen(this, new SelectScreen(engine, scene, inputProvider)),
                    TextDefaultColor = Colors.AlternateTextColor
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 128.0f)));

        return treeBuilder.Build();
    }

    public override void Update(UpdateArgs args)
    {
        root.UpdateRecursively(args);
    }

    public override void Render(RenderArgs args)
    {
        root.RenderRecursively(args);
    }

    public void Dispose()
    {
        root.DisposeRecursively();
    }
}