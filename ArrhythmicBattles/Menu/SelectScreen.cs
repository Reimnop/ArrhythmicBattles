using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Core;
using ArrhythmicBattles.Game;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;

namespace ArrhythmicBattles.Menu;

public class SelectScreen : Screen, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly ScopedInputProvider inputProvider;
    
    private readonly Node<ElementContainer> root;

    public SelectScreen(FlexFrameworkMain engine, ABScene scene, ScopedInputProvider inputProvider)
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
                .SetElement(new ABButtonElement(font, inputProvider, "SINGLEPLAYER")
                {
                    Click = () => engine.LoadScene(new GameScene(scene.Context))
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(0.0f, -64.0f, 0.0f, 0.0f))
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new ABButtonElement(font, inputProvider, "MULTIPLAYER")
                {
                    Click = () => scene.SwitchScreen(this, new MultiplayerScreen(engine, scene, inputProvider))
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 64.0f)))
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new ABButtonElement(font, inputProvider, "SETTINGS")
                {
                    Click = () => scene.SwitchScreen(this, new SettingsScreen(engine, scene, inputProvider))
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 128.0f)))
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new ABButtonElement(font, inputProvider, "CREDITS")
                {
                    Click = () => scene.SwitchScreen(this, new CreditsScreen(engine, scene, inputProvider))
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 192.0f)))
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new ABButtonElement(font, inputProvider, "EXIT")
                {
                    Click = () => scene.CloseScreen(this),
                    TextDefaultColor = Colors.AlternateTextColor
                })
                .SetAnchor(Anchor.FillTopEdge)
                .SetEdges(new Edges(0.0f, -64.0f, 0.0f, 0.0f).Translate(0.0f, 256.0f)));

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