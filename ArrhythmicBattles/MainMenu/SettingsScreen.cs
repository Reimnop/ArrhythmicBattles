using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;

namespace ArrhythmicBattles.MainMenu;

public class SettingsScreen : MenuScreen
{
    protected override Screen LastScreen => new SelectScreen(InputInfo, Engine, Scene);
    
    public SettingsScreen(InputInfo inputInfo, FlexFrameworkMain engine, MainMenuScene scene) : base(inputInfo, engine, scene)
    {
    }

    protected override void InitUI()
    {
        CreateButton("VIDEO", DefaultColor, () => { });
        CreateButton("AUDIO", DefaultColor, () => Scene.SwitchScreen(new AudioSettingsScreen(InputInfo, Engine, Scene)));
        CreateButton("BACK", ExitColor, () => Scene.SwitchScreen(LastScreen));
    }
}