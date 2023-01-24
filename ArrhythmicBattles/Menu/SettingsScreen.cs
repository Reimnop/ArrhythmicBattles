using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;

namespace ArrhythmicBattles.Menu;

public class SettingsScreen : MenuScreen
{
    protected override Screen LastScreen => new SelectScreen(Engine, Scene, InputInfo);
    
    public SettingsScreen(FlexFrameworkMain engine, ABScene scene, InputInfo inputInfo) : base(engine, scene, inputInfo)
    {
    }

    protected override void InitUI()
    {
        CreateButton("VIDEO", DefaultColor, () => { });
        CreateButton("AUDIO", DefaultColor, () => Scene.SwitchScreen(this, new AudioSettingsScreen(Engine, Scene, InputInfo)));
        CreateButton("BACK", ExitColor, () => Scene.SwitchScreen(this, LastScreen));
    }
}