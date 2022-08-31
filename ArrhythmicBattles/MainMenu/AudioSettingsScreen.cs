using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.MainMenu;

// TODO: Extract half of this to an abstract SettingsScreen class
public class AudioSettingsScreen : Screen
{
    private static readonly Vector2 ControlSize = new Vector2i(512, 56);
    private static readonly Color4 DefaultColor = Color4.White;
    private static readonly Color4 ExitColor = new Color4(233, 81, 83, 255);
    
    public override Vector2 Position
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

    private readonly List<UIElement> entities = new List<UIElement>();

    public AudioSettingsScreen(FlexFrameworkMain engine, MainMenuScene scene)
    {
        this.engine = engine;
        this.scene = scene;

        capture = scene.Context.InputSystem.AcquireCapture();
        inputInfo = new InputInfo(scene.Context.InputSystem, capture);
        
        // Controls
        CreateSlider("SFX VOLUME", scene.Context.Sound.SfxVolumeLevel, value => scene.Context.Sound.SfxVolumeLevel = value);
        CreateSlider("MUSIC VOLUME", scene.Context.Sound.MusicVolumeLevel, value => scene.Context.Sound.MusicVolumeLevel = value);
        CreateButton("BACK", ExitColor, () =>
        {
            scene.Context.SaveSettings();
            scene.SwitchScreen<SettingsScreen>(engine, scene);
        });
        
        // Layout stuff we shouldn't touch
        stackLayout = new VerticalStackLayout(engine);
        foreach (UIElement element in entities)
        {
            stackLayout.AddChild(element);
        }

        List<NavNode> navNodes = new List<NavNode>();
        foreach (UIElement element in entities)
        {
            navNodes.Add(new NavNode(element));
        }
        Utils.LinkNodesWrapAroundVertical(navNodes.ToArray());

        navigator = new KeyboardNavigator(inputInfo, navNodes[0]);
        navigator.OnNodeSelected += node => scene.Context.Sound.SelectSfx.Play();
    }

    private void CreateSlider(string text, int value, Action<int> valueChangedCallback)
    {
        SliderEntity sliderEntity = new SliderEntity(engine, inputInfo);
        sliderEntity.Value = value;
        sliderEntity.Text = text;
        sliderEntity.TextPosOffset = new Vector2i(16, 36);
        sliderEntity.BarPosOffset = new Vector2i(300, 16);
        sliderEntity.Size = ControlSize;
        sliderEntity.UnfocusedColor = DefaultColor;
        sliderEntity.FocusedColor = new Color4(33, 33, 33, 255);
        sliderEntity.OnValueChanged += valueChangedCallback;
        sliderEntity.OnValueChanged += value => scene.Context.Sound.SelectSfx.Play();
        entities.Add(sliderEntity);
    }
    
    private void CreateButton(string text, Color4 color, Action pressedCallback)
    {
        ButtonEntity buttonEntity = new ButtonEntity(engine, inputInfo)
            .WithText(text)
            .WithOrigin(0.0f, 1.0f)
            .WithTextPosOffset(16, 36)
            .WithSize(ControlSize)
            .WithTextUnfocusedColor(color)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .AddPressedCallback(() => scene.Context.Sound.SelectSfx.Play())
            .AddPressedCallback(pressedCallback);
        entities.Add(buttonEntity);
    }
    
    public override void Update(UpdateArgs args)
    {
        stackLayout.Update(args);
        navigator.Update(args);
        
        entities.ForEach(element => element.Update(args));
        
        if (scene.Context.InputSystem.GetKeyDown(capture, Keys.Escape))
        {
            scene.SwitchScreen<SettingsScreen>(engine, scene);
        }
    }
    
    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        navigator.Render(renderer, layerId, matrixStack, cameraData);
        
        entities.ForEach(element => (element as IRenderable)?.Render(renderer, layerId, matrixStack, cameraData));
    }

    public override void Dispose()
    {
        entities.ForEach(button => button.Dispose());
        
        capture.Dispose();
    }
}