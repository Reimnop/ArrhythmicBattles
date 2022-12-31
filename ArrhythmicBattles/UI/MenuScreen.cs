using ArrhythmicBattles.MainMenu;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.UI;

public abstract class MenuScreen : Screen
{
    protected static Vector2 ControlSize => new Vector2i(512, 56);
    protected static Color4 DefaultColor => Color4.White;
    protected static Color4 ExitColor => new Color4(233, 81, 83, 255);
    
    protected abstract Screen? LastScreen { get; }
    
    public override Vector2 Position
    {
        get => stackLayout.Position;
        set => stackLayout.Position = value;
    }
    
    protected InputInfo InputInfo { get; }
    protected FlexFrameworkMain Engine { get; }
    protected MainMenuScene Scene { get; }

    private readonly VerticalStackLayout stackLayout;
    private readonly KeyboardNavigator navigator;

    private readonly List<UIElement> elements = new List<UIElement>();

    public MenuScreen(InputInfo inputInfo, FlexFrameworkMain engine, MainMenuScene scene)
    {
        this.InputInfo = inputInfo;
        this.Engine = engine;
        this.Scene = scene;

        InitUI();
        
        // Layout stuff we shouldn't touch
        stackLayout = new VerticalStackLayout(engine);
        foreach (UIElement element in elements)
        {
            stackLayout.AddChild(element);
        }

        List<NavNode> navNodes = new List<NavNode>();
        foreach (UIElement element in elements)
        {
            navNodes.Add(new NavNode(element));
        }

        Utils.LinkNodesWrapAroundVertical(navNodes.ToArray());

        navigator = new KeyboardNavigator(inputInfo, navNodes[0]);
        navigator.OnNodeSelected += node => scene.Context.Sound.SelectSfx.Play();
    }

    protected abstract void InitUI();
    
    protected void CreateButton(string text, Color4 color, Action pressedCallback)
    {
        ButtonEntity buttonEntity = new ButtonEntity(Engine, InputInfo);
        buttonEntity.Text = text;
        buttonEntity.Origin = new Vector2(0.0f, 1.0f);
        buttonEntity.TextPosOffset = new Vector2i(16, 36);
        buttonEntity.Size = ControlSize;
        buttonEntity.TextUnfocusedColor = color;
        buttonEntity.TextFocusedColor = new Color4(33, 33, 33, 255);
        buttonEntity.Pressed += () => Scene.Context.Sound.SelectSfx.Play();
        buttonEntity.Pressed += pressedCallback;
        elements.Add(buttonEntity);
    }
    protected void CreateSlider(string text, int value, Action<int> valueChangedCallback)
    {
        SliderEntity sliderEntity = new SliderEntity(Engine, InputInfo);
        sliderEntity.Value = value;
        sliderEntity.Text = text;
        sliderEntity.TextPosOffset = new Vector2i(16, 36);
        sliderEntity.BarPosOffset = new Vector2i(300, 16);
        sliderEntity.Size = ControlSize;
        sliderEntity.UnfocusedColor = DefaultColor;
        sliderEntity.FocusedColor = new Color4(33, 33, 33, 255);
        sliderEntity.OnValueChanged += valueChangedCallback;
        sliderEntity.OnValueChanged += _ => Scene.Context.Sound.SelectSfx.Play();
        elements.Add(sliderEntity);
    }

    public override void Update(UpdateArgs args)
    {
        stackLayout.Update(args);
        navigator.Update(args);
        
        elements.ForEach(element => element.Update(args));
        
        if (LastScreen != null && Scene.Context.InputSystem.GetKeyDown(InputInfo.InputCapture, Keys.Escape))
        {
            Scene.SwitchScreen(LastScreen);
        }
    }
    
    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        navigator.Render(renderer, layerId, matrixStack, cameraData);
        elements.ForEach(element => (element as IRenderable)?.Render(renderer, layerId, matrixStack, cameraData));
    }

    public override void Dispose()
    {
        elements.ForEach(button => button.Dispose());
    }
}