using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.UserInterface;

namespace ArrhythmicBattles.Menu;

public class SettingsScreen : MenuScreen
{
    protected override Screen LastScreen => new SelectScreen(Engine, Scene, InputProvider);
    
    public SettingsScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider) : base(engine, scene, inputProvider)
    {
    }

    protected override void InitUI()
    {
        CreateButton("VIDEO", DefaultColor, () => { });
        CreateButton("AUDIO", DefaultColor, () => Scene.SwitchScreen(this, new AudioSettingsScreen(Engine, Scene, InputProvider)));
        CreateButton("BACK", ExitColor, () => Scene.SwitchScreen(this, LastScreen));
    }
}