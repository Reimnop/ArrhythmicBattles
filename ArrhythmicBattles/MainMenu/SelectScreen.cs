using ArrhythmicBattles.MainGame;
using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;

namespace ArrhythmicBattles.MainMenu;

public class SelectScreen : MenuScreen
{
    protected override Screen? LastScreen => null;
    
    public SelectScreen(InputInfo inputInfo, FlexFrameworkMain engine, MainMenuScene scene) : base(inputInfo, engine, scene)
    {
    }

    protected override void InitUI()
    {
        CreateButton("SINGLEPLAYER", DefaultColor, () => Scene.LoadScene(new GameScene(Scene.Context)));
        CreateButton("MULTIPLAYER", DefaultColor, () => { });
        CreateButton("SETTINGS", DefaultColor, () => Scene.SwitchScreen(new SettingsScreen(InputInfo, Engine, Scene)));
        CreateButton("CREDITS", DefaultColor, () => Scene.SwitchScreen(new CreditsScreen(InputInfo, Engine, Scene)));
        CreateButton("EXIT", ExitColor, () => Engine.Close());
    }
}