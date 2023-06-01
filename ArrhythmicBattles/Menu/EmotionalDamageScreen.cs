using System.Diagnostics;
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

public class EmotionalDamageScreen : Screen, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly ScopedInputProvider inputProvider;
    
    private readonly Node<ElementContainer> root;

    public EmotionalDamageScreen(FlexFrameworkMain engine, ABScene scene, ScopedInputProvider inputProvider)
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
            .SetWidth(StretchMode.Stretch)
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new TextElement(font)
                {
                    Text = "I can't believe you fell for that\nHow stupid are you?"
                })
                .SetWidth(StretchMode.Stretch))
            .AddChild(new InterfaceTreeBuilder()
                .SetElement(new ABButtonElement(font, inputProvider, "BACK")
                {
                    Click = () => engine.Close()
                })
                .SetWidth(StretchMode.Stretch)
                .SetPadding(16.0f)
                .SetMargin(16.0f, 0.0f, 0.0f, 0.0f)
                .SetHeight(80.0f));

        return treeBuilder.Build();
    }

    public override void Update(UpdateArgs args)
    {
        foreach (var updatable in root.Select(x => x.Value.Element).OfType<IUpdateable>())
        {
            updatable.Update(args);
        }
        
        // TODO: Uncomment this when SelectScreen is implemented
        // if (inputProvider.GetKeyDown(Keys.Escape))
        // {
        //     scene.SwitchScreen(this, new SelectScreen(engine, scene, inputProvider));
        // }
    }

    public override void Render(RenderArgs args)
    {
        foreach (var renderable in root.Select(x => x.Value.Element).OfType<IRenderable>())
        {
            renderable.Render(args);
        }
    }

    public void Dispose()
    {
        foreach (var disposable in root.Select(x => x.Value.Element).OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}