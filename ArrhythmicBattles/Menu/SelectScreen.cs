using ArrhythmicBattles.Game;
using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.UserInterface;

namespace ArrhythmicBattles.Menu;

public class SelectScreen : MenuScreen
{
    protected override Screen? LastScreen => null;
    
    public SelectScreen(FlexFrameworkMain engine, ABScene scene, IInputProvider inputProvider) : base(engine, scene, inputProvider)
    {
    }

    protected override void InitUI()
    {
        CreateButton("SINGLEPLAYER", DefaultColor, () => Engine.LoadScene(new GameScene(Scene.Context)));
        CreateButton("MULTIPLAYER", DefaultColor, () => Scene.SwitchScreen(this, new MultiplayerScreen(Engine, Scene, InputProvider)));
        CreateButton("SETTINGS", DefaultColor, () => Scene.SwitchScreen(this, new SettingsScreen(Engine, Scene, InputProvider)));
        CreateButton("CREDITS", DefaultColor, () => Scene.SwitchScreen(this, new CreditsScreen(Engine, Scene, InputProvider)));
        CreateButton("EXIT", ExitColor, () => Scene.CloseScreen(this));
    }
}