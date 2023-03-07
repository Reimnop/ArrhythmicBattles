using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Menu;

public class AudioSettingsScreen : Screen, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    private readonly IInputProvider inputProvider;
    
    private readonly Element root;

    public AudioSettingsScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider)
    {
        this.engine = engine;
        this.scene = scene;
        this.inputProvider = inputProvider;

        root = BuildInterface();
        root.UpdateLayout(scene.ScreenBounds);
    }

    private Element BuildInterface()
    {
        return new StackLayoutElement(
            Direction.Vertical,
            new ABSliderElement(engine, inputProvider, "SFX VOLUME")  
            {
                Value = scene.Context.Sound.SfxVolumeLevel,
                ValueChanged = value => scene.Context.Sound.SfxVolumeLevel = value,
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            },
            new ABSliderElement(engine, inputProvider, "MUSIC VOLUME")
            {
                Value = scene.Context.Sound.MusicVolumeLevel,
                ValueChanged = value => scene.Context.Sound.MusicVolumeLevel = value,
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel)
            },
            new ABButtonElement(engine, inputProvider, "BACK")
            {
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(16.0f, Unit.Pixel),
                TextDefaultColor = new Color4(233, 81, 83, 255),
                Click = () => scene.SwitchScreen(this, new SettingsScreen(engine, scene, inputProvider))
            })
        {
            Width = Length.Full
        };
    }

    public override void Update(UpdateArgs args)
    {
        root.UpdateRecursive(args);
        
        if (inputProvider.GetKeyDown(Keys.Escape))
        {
            scene.SwitchScreen(this, new SettingsScreen(engine, scene, inputProvider));
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