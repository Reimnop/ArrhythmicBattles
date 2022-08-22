using ArrhythmicBattles.MainGame;
using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.MainMenu;

public class SelectScreen : Screen
{
    private static readonly Vector2i ButtonSize = new Vector2i(512, 56);
    private static readonly Color4 DefaultColor = Color4.White;
    private static readonly Color4 ExitColor = new Color4(233, 81, 83, 255);
    
    public override Vector2i Position
    {
        get => stackLayout.Position;
        set => stackLayout.Position = value;
    }
    
    private readonly FlexFrameworkMain engine;
    private readonly MainMenuScene scene;
    private readonly VerticalStackLayout stackLayout;
    private readonly KeyboardNavigator navigator;
    private readonly InputCapture capture;
    private readonly InputInfo inputInfo;

    private readonly List<ButtonEntity> buttonEntities = new List<ButtonEntity>();

    public SelectScreen(FlexFrameworkMain engine, MainMenuScene scene)
    {
        this.engine = engine;
        this.scene = scene;
        
        capture = scene.Context.InputSystem.AcquireCapture();
        inputInfo = new InputInfo(scene.Context.InputSystem, capture);
        
        // Buttons
        CreateButton("SINGLEPLAYER", DefaultColor, () => scene.LoadScene<GameScene>(scene.Context, scene.SfxContext));
        CreateButton("MULTIPLAYER", DefaultColor, () => { });
        CreateButton("SETTINGS", DefaultColor, () => scene.SwitchScreen<SettingsScreen>(engine, scene));
        CreateButton("CREDITS", DefaultColor, () => scene.SwitchScreen<CreditsScreen>(engine, scene));
        CreateButton("EXIT", ExitColor, () => engine.Close());
        
        // Layout stuff we shouldn't touch
        stackLayout = new VerticalStackLayout(engine);
        foreach (ButtonEntity buttonEntity in buttonEntities)
        {
            stackLayout.AddChild(buttonEntity);
        }

        List<NavNode> navNodes = new List<NavNode>();
        foreach (ButtonEntity buttonEntity in buttonEntities)
        {
            navNodes.Add(new NavNode(buttonEntity));
        }
        Utils.LinkNodesWrapAroundVertical(navNodes.ToArray());

        navigator = new KeyboardNavigator(inputInfo, navNodes[0]);
        navigator.OnNodeSelected += node => scene.SfxContext.SelectSfx.Play();
    }

    private void CreateButton(string text, Color4 color, Action pressedCallback)
    {
        ButtonEntity buttonEntity = new ButtonEntity(engine, inputInfo)
            .WithText(text)
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextUnfocusedColor(color)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(ButtonSize)
            .AddPressedCallback(() => scene.SfxContext.SelectSfx.Play())
            .AddPressedCallback(pressedCallback);
        buttonEntities.Add(buttonEntity);
    }
    
    public override void Update(UpdateArgs args)
    {
        stackLayout.Update(args);
        navigator.Update(args);
        
        buttonEntities.ForEach(button => button.Update(args));
    }
    
    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        navigator.Render(renderer, layerId, matrixStack, cameraData);
        
        buttonEntities.ForEach(button => button.Render(renderer, layerId, matrixStack, cameraData));
    }

    public override void Dispose()
    {
        buttonEntities.ForEach(button => button.Dispose());
        
        capture.Dispose();
    }
}