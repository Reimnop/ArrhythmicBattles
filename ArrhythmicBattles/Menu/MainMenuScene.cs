using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using ArrhythmicBattles.Core;
using ArrhythmicBattles.Settings;
using FlexFramework.Core;
using FlexFramework.Core.Audio;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.PostProcessing;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Modelling;
using FlexFramework.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace ArrhythmicBattles.Menu;

public class MainMenuScene : ABScene, IDisposable
{
    // Background
    private Quaternion backgroundRotation = Quaternion.Identity;
    private Vector2 mousePositionSmoothed = Vector2.Zero;
    
    private readonly Model backgroundModel;
    private readonly ModelEntity backgroundEntity;
    private readonly Fxaa fxaa;
    private readonly Bloom bloom;
    private readonly Aces tonemapper;

    private readonly PerspectiveCamera camera = new()
    {
        // Magic numbers... It just looks good, okay?
        Position = new Vector3(-2.0f, 0.0f, 15.0f),
        Fov = MathHelper.DegreesToRadians(40.0f)
    };

    // Other things
    private readonly GuiCamera guiCamera = new();
    private readonly EntityManager entityManager = new();
    private readonly MatrixStack matrixStack = new();
    private readonly AudioSource musicAudioSource;
    private readonly AudioSource sfxAudioSource;
    private readonly ScreenManager screenManager;
    private readonly ScopedInputProvider inputProvider;
    
    private readonly Binding<float> musicVolumeBinding;
    private readonly Binding<float> sfxVolumeBinding;
    
    private Box2 currentScreenBounds;

    public MainMenuScene(ABContext context) : base(context)
    {
        Engine.CursorState = CursorState.Normal;
        currentScreenBounds = new Box2(Vector2.Zero, Engine.ClientSize);

        // Init audio
        var resourceManager = Context.ResourceManager;
        var settings = Context.Settings;

        musicAudioSource = new AudioSource
        {
            AudioStream = resourceManager.Get<AudioStream>("Audio/Arrhythmic.ogg")
        };
        musicAudioSource.Play();

        sfxAudioSource = new AudioSource
        {
            Looping = false,
            AudioStream = resourceManager.Get<AudioStream>("Audio/Select.ogg")
        };

        // Init bindings
        musicVolumeBinding = new Binding<float>(settings, nameof(ISettings.MusicVolume), musicAudioSource, nameof(AudioSource.Gain));
        sfxVolumeBinding = new Binding<float>(settings, nameof(ISettings.SfxVolume), sfxAudioSource, nameof(AudioSource.Gain));
        
        // Init input
        inputProvider = Context.InputSystem.AcquireInputProvider();
        
        // Init background
        backgroundModel = resourceManager.Get<Model>("Models/LogoBackground.fbx");
        backgroundEntity = entityManager.Create(() => new ModelEntity(backgroundModel));
        fxaa = new Fxaa();
        bloom = new Bloom()
        {
            Strength = 0.85f
        };
        tonemapper = new Aces();

        // Init UI
        var bannerTexture = resourceManager.Get<TextureSampler>("Textures/Banner.png");
        var boldFont = resourceManager.Get<Font>(Constants.BoldFontPath);
        
        // Magic numbers retrieved from design in Figma
        screenManager = new ScreenManager(currentScreenBounds, Engine.DpiScale, child => 
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder() // Header
                    .SetElement(new ImageElement(bannerTexture))
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(16.0f, -144.0f, 16.0f, -720.0f))
                .AddChild(new InterfaceTreeBuilder() // Footer
                    .SetElement(new RectElement()
                    {
                        Color = Color4.White,
                        Radius = 8.0f,
                        BorderThickness = 2.0f
                    })
                    .SetAnchor(Anchor.FillBottomEdge)
                    .SetEdges(-80.0f, 16.0f, 16.0f, 16.0f)
                    .AddChild(new InterfaceTreeBuilder()
                        .SetElement(new TextElement(boldFont)
                        {
                            Color = Color4.White,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,
                            EmSize = 32.0f / 24.0f,
                            Text = Constants.CompanyName
                        })
                        .SetAnchor(Anchor.Fill)
                        .SetEdges(16.0f, 16.0f, 16.0f, 16.0f))
                    .AddChild(new InterfaceTreeBuilder()
                        .SetElement(new TextElement(boldFont)
                        {
                            Color = Color4.White,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            EmSize = 32.0f / 24.0f,
                            Text = Constants.GameVersion
                        })
                        .SetAnchor(Anchor.Fill)
                        .SetEdges(16.0f, 16.0f, 16.0f, 16.0f)))
                .AddChild(new InterfaceTreeBuilder() // Body
                    .SetElement(new RectElement()
                        {
                            Color = Color4.White,
                            Radius = 8.0f,
                            BorderThickness = 2.0f
                        })
                    .SetAnchor(Anchor.Fill)
                    .SetEdges(160.0f, 96.0f, 16.0f, 16.0f)
                    .AddChild(child)) 
            );
        
        screenManager.Open(new MainScreen(Context, screenManager, inputProvider));

        screenManager.SwitchScreen += (_, _) => sfxAudioSource.Play();
        screenManager.CloseScreen += _ =>
        {
            if (screenManager.Screens.Count == 0)
                Engine.Close();
        };
    }

    public override void Update(UpdateArgs args)
    {
        // Update background rotation
        var backgroundYaw = MathHelper.DegreesToRadians(MathF.Sin(args.Time * 0.25f * MathF.PI) * 15.0f);
        var backgroundPitch = MathHelper.DegreesToRadians(MathF.Sin(args.Time * 0.0625f * MathF.PI) * 15.0f);
        var backgroundRoll = MathHelper.DegreesToRadians(MathF.Sin(args.Time * 0.125f * MathF.PI) * 15.0f);
        
        var mousePosition = inputProvider.MousePosition;
        mousePosition.X = Math.Clamp(mousePosition.X, 0.0f, Engine.ClientSize.X);
        mousePosition.Y = Math.Clamp(mousePosition.Y, 0.0f, Engine.ClientSize.Y);
        mousePositionSmoothed = Vector2.Lerp(mousePositionSmoothed, mousePosition, args.DeltaTime * 6.0f);
        var mouseYNormalized = mousePositionSmoothed.Y / Engine.ClientSize.Y * 2.0f - 1.0f;
        var mouseXNormalized = mousePositionSmoothed.X / Engine.ClientSize.X * 2.0f - 1.0f;
        backgroundYaw += MathHelper.DegreesToRadians(mouseXNormalized * 20.0f);
        backgroundPitch += MathHelper.DegreesToRadians(mouseYNormalized * 20.0f);

        backgroundRotation = Quaternion.FromEulerAngles(backgroundPitch, backgroundYaw, backgroundRoll);
        
        var screenBounds = new Box2(Vector2.Zero, Engine.ClientSize);
        
        if (currentScreenBounds != screenBounds)
        {
            currentScreenBounds = screenBounds;
            screenManager.Resize(currentScreenBounds, Engine.DpiScale);
        }
        
        screenManager.Update(args);
    }

    protected override void RenderScene(CommandList commandList)
    {
        commandList.AddPostProcessor(fxaa);
        commandList.AddPostProcessor(bloom);
        commandList.AddPostProcessor(tonemapper);

        var cameraData = camera.GetCameraData(Engine.ClientSize);
        var args = new RenderArgs(commandList, LayerType.Opaque, matrixStack, cameraData);
        
        matrixStack.Push();
        matrixStack.Rotate(backgroundRotation);
        backgroundEntity.Render(args);
        matrixStack.Pop();
        
        var guiCameraData = guiCamera.GetCameraData((Vector2i) screenManager.ComputedBounds.Size);
        var guiArgs = new RenderArgs(commandList, LayerType.Gui, matrixStack, guiCameraData);
        screenManager.Render(guiArgs);
    }

    public void Dispose()
    {
        entityManager.Dispose();
        musicVolumeBinding.Dispose();
        sfxVolumeBinding.Dispose();
        inputProvider.Dispose();
        musicAudioSource.Dispose();
        sfxAudioSource.Dispose();
        screenManager.Dispose();
        backgroundModel.Dispose();
        fxaa.Dispose();
        bloom.Dispose();
        tonemapper.Dispose();
    }
}