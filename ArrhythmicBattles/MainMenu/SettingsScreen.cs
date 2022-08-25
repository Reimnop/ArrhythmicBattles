using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainMenu;

public class SettingsScreen : Screen
{
    private static readonly Color4 DefaultColor = Color4.White;
    private static readonly Color4 ExitColor = new Color4(233, 81, 83, 255);
    
    public override Vector2d Position
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

    public SettingsScreen(FlexFrameworkMain engine, MainMenuScene scene)
    {
        this.engine = engine;
        this.scene = scene;

        capture = scene.Context.InputSystem.AcquireCapture();
        inputInfo = new InputInfo(scene.Context.InputSystem, capture);
        
        // Buttons
        CreateButton("VIDEO", DefaultColor, () => { });
        CreateButton("AUDIO", DefaultColor, () => scene.SwitchScreen<AudioSettingsScreen>(engine, scene));
        CreateButton("BACK", ExitColor, () => scene.SwitchScreen<SelectScreen>(engine, scene));
        
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
        navigator.OnNodeSelected += node => scene.Context.Sound.SelectSfx.Play();
    }
    
    private void CreateButton(string text, Color4 color, Action pressedCallback)
    {
        ButtonEntity buttonEntity = new ButtonEntity(engine, inputInfo)
            .WithText(text)
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(16, 36)
            .WithSize(512, 56)
            .WithTextUnfocusedColor(color)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .AddPressedCallback(() => scene.Context.Sound.SelectSfx.Play())
            .AddPressedCallback(pressedCallback);
        buttonEntities.Add(buttonEntity);
    }
    
    public override void Update(UpdateArgs args)
    {
        stackLayout.Update(args);
        navigator.Update(args);
        
        buttonEntities.ForEach(button => button.Update(args));
        
        if (scene.Context.InputSystem.GetKeyDown(capture, Keys.Escape))
        {
            scene.SwitchScreen<SelectScreen>(engine, scene);
        }
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