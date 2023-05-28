using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Core;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Menu;

public class MultiplayerScreen : Screen, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly ScopedInputProvider inputProvider;
    
    private readonly Element root;

    public MultiplayerScreen(FlexFrameworkMain engine, ABScene scene, ScopedInputProvider inputProvider)
    {
        this.engine = engine;
        this.scene = scene;
        this.inputProvider = inputProvider;

        root = BuildInterface();
        root.UpdateLayout(scene.ScreenBounds);
    }

    private Element BuildInterface()
    {
        var font = scene.Context.Font;
        
        return new StackLayoutElement(
            Direction.Vertical,
            new SelectableTextElement(font, inputProvider)
            {
                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor\n" +
                       "incididunt ut labore et dolore magna aliqua. Nunc non blandit massa enim nec. Eu mi\n" +
                       "bibendum neque egestas congue quisque egestas diam in. Id neque aliquam vestibulum\n" +
                       "morbi blandit cursus risus at ultrices. Risus at ultrices mi tempus imperdiet. Arcu\n" +
                       "ac tortor dignissim convallis aenean et tortor. Vel quam elementum pulvinar etiam\n" +
                       "non quam lacus. At quis risus sed vulputate. Pulvinar sapien et ligula ullamcorper\n" +
                       "malesuada proin libero. In ante metus dictum at tempor commodo ullamcorper. Cras semper\n" +
                       "auctor neque vitae tempus quam pellentesque. Lectus sit amet est placerat in egestas\n" +
                       "erat imperdiet sed. Facilisis volutpat est velit egestas. Leo vel fringilla est\n" +
                       "ullamcorper eget nulla facilisi etiam dignissim. Enim eu turpis egestas pretium aenean\n" +
                       "pharetra. Egestas tellus rutrum tellus pellentesque eu. Eros donec ac odio tempor orci\n" +
                       "dapibus ultrices in iaculis. Mauris augue neque gravida in fermentum et. Quisque\n" +
                       "sagittis purus sit amet.",
                Width = Length.Full
            },
            new ABButtonElement(font, inputProvider, "BACK")
            {
                TextDefaultColor = new Color4(233, 81, 83, 255),
                Width = Length.Full,
                Height = 64.0f,
                Padding = 16.0f,
                Click = () => scene.SwitchScreen(this, new SelectScreen(engine, scene, inputProvider))
            })
        {
            Width = Length.Full,
            Spacing = 16.0f
        };
    }

    public override void Update(UpdateArgs args)
    {
        root.UpdateRecursive(args);
        
        if (inputProvider.GetKeyDown(Keys.Escape))
        {
            scene.SwitchScreen(this, new SelectScreen(engine, scene, inputProvider));
        }
    }

    public override void Render(RenderArgs args)
    {
        root.RenderRecursive(args);
    }

    public void Dispose()
    {
        root.DisposeRecursive();
    }
}