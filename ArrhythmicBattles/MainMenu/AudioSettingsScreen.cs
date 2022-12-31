using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;

namespace ArrhythmicBattles.MainMenu;

public class AudioSettingsScreen : MenuScreen
{
    protected override Screen LastScreen => new SettingsScreen(InputInfo, Engine, Scene);
    
    public AudioSettingsScreen(InputInfo inputInfo, FlexFrameworkMain engine, MainMenuScene scene) : base(inputInfo, engine, scene)
    {
    }

    protected override void InitUI()
    {
        CreateSlider("SFX VOLUME", Scene.Context.Sound.SfxVolumeLevel, value => Scene.Context.Sound.SfxVolumeLevel = value);
        CreateSlider("MUSIC VOLUME", Scene.Context.Sound.MusicVolumeLevel, value => Scene.Context.Sound.MusicVolumeLevel = value);
        CreateButton("BACK", ExitColor, () =>
        {
            Scene.Context.SaveSettings();
            Scene.SwitchScreen(LastScreen);
        });
    }
}