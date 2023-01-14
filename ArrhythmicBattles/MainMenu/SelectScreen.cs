using ArrhythmicBattles.MainGame;
using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;

namespace ArrhythmicBattles.MainMenu;

public class SelectScreen : MenuScreen
{
    protected override Screen? LastScreen => null;
    
    public SelectScreen(FlexFrameworkMain engine, ABScene scene, InputInfo inputInfo) : base(engine, scene, inputInfo)
    {
    }

    protected override void InitUI()
    {
        CreateButton("SINGLEPLAYER", DefaultColor, () => Engine.LoadScene(new GameScene(Scene.Context)));
        CreateButton("MULTIPLAYER", DefaultColor, () => { });
        CreateButton("SETTINGS", DefaultColor, () => Scene.SwitchScreen(this, new SettingsScreen(Engine, Scene, InputInfo)));
        CreateButton("CREDITS", DefaultColor, () => Scene.SwitchScreen(this, new CreditsScreen(Engine, Scene, InputInfo)));
        CreateButton("EXIT", ExitColor, () => Scene.CloseScreen(this));
    }
}