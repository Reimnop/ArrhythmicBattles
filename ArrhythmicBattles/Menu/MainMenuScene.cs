using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using ArrhythmicBattles.Core;
using ArrhythmicBattles.Settings;
using FlexFramework.Core;
using FlexFramework.Core.Audio;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace ArrhythmicBattles.Menu;

public class MainMenuScene : ABScene
{
    // Other things
    private readonly AudioStream musicAudioStream;
    private readonly AudioSource musicAudioSource;
    private readonly AudioStream sfxAudioStream;
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

        // Reset lightings
        if (Engine.Renderer is ILighting lightings)
        {
            lightings.DirectionalLight = null;
        }
        
        // Init audio
        var settings = Context.Settings;
        
        musicAudioStream = new VorbisAudioStream("Assets/Audio/Arrhythmic.ogg");
        musicAudioSource = new AudioSource();
        musicAudioSource.Gain = settings.MusicVolume;
        musicAudioSource.AudioStream = musicAudioStream;
        musicAudioSource.Play();

        sfxAudioStream = new VorbisAudioStream("Assets/Audio/Select.ogg");
        sfxAudioSource = new AudioSource();
        sfxAudioSource.Gain = settings.SfxVolume;
        sfxAudioSource.Looping = false;
        sfxAudioSource.AudioStream = sfxAudioStream;

        // Init bindings
        musicVolumeBinding = new Binding<float>(settings, nameof(ISettings.MusicVolume), musicAudioSource, nameof(AudioSource.Gain));
        sfxVolumeBinding = new Binding<float>(settings, nameof(ISettings.SfxVolume), sfxAudioSource, nameof(AudioSource.Gain));
        
        // Init input
        inputProvider = Context.InputSystem.AcquireInputProvider();

        // Init UI
        var bannerPath = RandomHelper.RandomFromTime() < 0.002 ? "Assets/banner_alt.png" : "Assets/banner.png"; // Sneaky easter egg
        var bannerTexture = Texture.FromFile("banner", bannerPath);
        var font = Context.Font;
        
        screenManager = new ScreenManager(currentScreenBounds, child => 
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new RectElement()
                    {
                        Color = Colors.Surface
                    })
                    .SetAnchor(Anchor.FillTopEdge)
                    .SetEdges(0.0f, -192.0f, 0.0f, 0.0f)
                    .AddChild(new InterfaceTreeBuilder()
                        .SetElement(new ImageElement(bannerTexture)
                        {
                            ImageMode = ImageMode.Fit
                        })
                        .SetAnchor(Anchor.Fill)
                        .SetEdges(32.0f)))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new RectElement()
                    {
                        Color = Colors.Surface
                    })
                    .SetAnchor(Anchor.FillBottomEdge)
                    .SetEdges(-64.0f, 0.0f, 0.0f, 0.0f)
                    .AddChild(new InterfaceTreeBuilder()
                        .SetElement(new TextElement(font)
                        {
                            Text = $"Version 0.0.1 BETA\n© {DateTime.Now.Year} Arrhythmic Battles",
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            EmSize = 18.0f / 24.0f
                        })
                        .SetAnchor(Anchor.Fill)
                        .SetEdges(12.0f, 12.0f, 12.0f, 12.0f)))
                .AddChild(new InterfaceTreeBuilder()
                    .SetAnchor(Anchor.Fill)
                    .SetEdges(224.0f, 96.0f, 32.0f, 32.0f)
                    .AddChild(child)) // Screen elements will be added here
            );
        
        screenManager.Open(new SelectScreen(Engine, screenManager, Context, inputProvider));

        screenManager.SwitchScreen += (_, _) => sfxAudioSource.Play();
        screenManager.CloseScreen += _ =>
        {
            if (screenManager.Screens.Count == 0)
                Engine.Close();
        };
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        var screenBounds = new Box2(Vector2.Zero, Engine.ClientSize);
        
        if (currentScreenBounds != screenBounds)
        {
            currentScreenBounds = screenBounds;
            screenManager.Resize(currentScreenBounds);
        }
        
        screenManager.Update(args);
    }

    protected override void RenderScene(CommandList commandList)
    {
        var cameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        var args = new RenderArgs(commandList, LayerType.Gui, MatrixStack, cameraData);
        
        screenManager.Render(args);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        musicVolumeBinding.Dispose();
        sfxVolumeBinding.Dispose();
        
        inputProvider.Dispose();
        musicAudioSource.Dispose();
        musicAudioStream.Dispose();
        sfxAudioSource.Dispose();
        sfxAudioStream.Dispose();
        
        screenManager.Dispose();
    }
}