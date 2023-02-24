using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.UserInterface;

namespace ArrhythmicBattles.Menu;

public class AudioSettingsScreen : MenuScreen
{
    protected override Screen LastScreen => new SettingsScreen(Engine, Scene, InputProvider);
    
    public AudioSettingsScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider) : base(engine, scene, inputProvider)
    {
    }

    protected override void InitUI()
    {
        CreateSlider("SFX VOLUME", Scene.Context.Sound.SfxVolumeLevel, value => Scene.Context.Sound.SfxVolumeLevel = value);
        CreateSlider("MUSIC VOLUME", Scene.Context.Sound.MusicVolumeLevel, value => Scene.Context.Sound.MusicVolumeLevel = value);
        CreateButton("BACK", ExitColor, () =>
        {
            Scene.Context.SaveSettings();
            Scene.SwitchScreen(this, LastScreen);
        });
    }
}