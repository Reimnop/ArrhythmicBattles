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
    
    public override Vector2i Position
    {
        get => stackLayout.Position;
        set => stackLayout.Position = value;
    }

    private readonly VerticalStackLayout stackLayout;
    private readonly KeyboardNavigator navigator;
    private readonly EntityGroup entityGroup;
    private readonly InputCapture capture;

    public SelectScreen(FlexFrameworkMain engine, MainMenuScene scene)
    {
        capture = scene.Context.InputSystem.AcquireCapture();
        InputInfo inputInfo = new InputInfo(scene.Context.InputSystem, capture);
        
        entityGroup = new EntityGroup();

        ButtonEntity singleplayerButton = new ButtonEntity(engine, inputInfo)
            .WithText("SINGLEPLAYER")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(ButtonSize)
            .AddPressedCallback(() => scene.SfxContext.SelectSfx.Play())
            .AddPressedCallback(() => scene.LoadScene<GameScene>(scene.Context, scene.SfxContext));

        ButtonEntity multiplayerButton = new ButtonEntity(engine, inputInfo)
            .WithText("MULTIPLAYER")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(ButtonSize)
            .AddPressedCallback(() => scene.SfxContext.SelectSfx.Play());
        
        ButtonEntity settingsButton = new ButtonEntity(engine, inputInfo)
            .WithText("SETTINGS")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(ButtonSize)
            .AddPressedCallback(() => scene.SfxContext.SelectSfx.Play())
            .AddPressedCallback(() => scene.SwitchScreen<SettingsScreen>(engine, scene));

        ButtonEntity exitButton = new ButtonEntity(engine, inputInfo)
            .WithText("EXIT")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextUnfocusedColor(new Color4(233, 81, 83, 255))
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(ButtonSize)
            .AddPressedCallback(() => scene.SfxContext.SelectSfx.Play())
            .AddPressedCallback(() => engine.Close());

        stackLayout = new VerticalStackLayout(engine)
            .WithPosition(48, 304);

        stackLayout.AddChild(singleplayerButton);
        stackLayout.AddChild(multiplayerButton);
        stackLayout.AddChild(settingsButton);
        stackLayout.AddChild(exitButton);
        
        entityGroup.AddEntity(singleplayerButton, multiplayerButton, settingsButton, exitButton);
        
        NavNode playNode = new NavNode(singleplayerButton);
        NavNode multiplayerNode = new NavNode(multiplayerButton);
        NavNode configNode = new NavNode(settingsButton);
        NavNode exitNode = new NavNode(exitButton);

        playNode.Top = exitNode;
        playNode.Bottom = multiplayerNode;
        multiplayerNode.Top = playNode;
        multiplayerNode.Bottom = configNode;
        configNode.Top = multiplayerNode;
        configNode.Bottom = exitNode;
        exitNode.Top = configNode;
        exitNode.Bottom = playNode;

        navigator = new KeyboardNavigator(inputInfo, playNode);
        navigator.OnNodeSelected += node => scene.SfxContext.SelectSfx.Play();
    }
    
    public override void Update(UpdateArgs args)
    {
        stackLayout.Update(args);
        navigator.Update(args);
        entityGroup.Update(args);
    }
    
    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        navigator.Render(renderer, layerId, matrixStack, cameraData);
        entityGroup.Render(renderer, layerId, matrixStack, cameraData);
    }

    public override void Dispose()
    {
        entityGroup.Dispose();
        capture.Dispose();
    }
}