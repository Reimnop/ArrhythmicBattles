using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using ArrhythmicBattles.Core;
using ArrhythmicBattles.Settings;
using FlexFramework.Core;
using FlexFramework.Core.Audio;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Text;
using FlexFramework.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace ArrhythmicBattles.Menu;

public class MainMenuScene : ABScene
{
    public override Box2 ScreenBounds => new(48.0f, 306.0f, 816.0f, 306.0f);

    // Resources
    private readonly Texture bannerTexture;
    
    // Entities
    private readonly ImageEntity bannerEntity;
    private readonly TextEntity copyrightText;
    private readonly MeshEntity border;

    // Other things
    private readonly AudioStream musicAudioStream;
    private readonly AudioSource musicAudioSource;
    private readonly AudioStream sfxAudioStream;
    private readonly AudioSource sfxAudioSource;
    private readonly ScopedInputProvider inputProvider;
    
    private readonly Binding<float> musicVolumeBinding;
    private readonly Binding<float> sfxVolumeBinding;

    public MainMenuScene(ABContext context) : base(context)
    {
        Engine.CursorState = CursorState.Normal;

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

        // Init resources
        string bannerPath = RandomHelper.RandomFromTime() < 0.002 ? "Assets/banner_alt.png" : "Assets/banner.png"; // Sneaky easter egg
        bannerTexture = Texture.FromFile("banner", bannerPath);
        
        // Init input
        inputProvider = Context.InputSystem.AcquireInputProvider();

        // Init entities
        bannerEntity = EntityManager.Create(() => new ImageEntity(bannerTexture));
        bannerEntity.Size = new Vector2(0.0f, 192.0f);
        bannerEntity.ImageMode = ImageMode.Stretch;

        var font = Context.Font;
        
        copyrightText = EntityManager.Create(() => new TextEntity(font));
        copyrightText.EmSize = 18.0f / 24.0f;
        copyrightText.HorizontalAlignment = HorizontalAlignment.Right;
        copyrightText.Text = $"Version 0.0.1 BETA\n© {DateTime.Now.Year} Arrhythmic Battles";
        // copyrightText.Text = "Luce, do not.\nLuce, your status.";

        border = EntityManager.Create(() => new MeshEntity(DefaultAssets.QuadMesh));
        border.Color = new Color4(24, 24, 24, 255);

        // Init UI
        OpenScreen(new SelectScreen(Engine, this, inputProvider));
    }

    public override void SwitchScreen(Screen before, Screen after)
    {
        base.SwitchScreen(before, after);
        
        sfxAudioSource.Play();
    }

    public override void CloseScreen(Screen screen)
    {
        base.CloseScreen(screen);

        if (ScreenHandler.Screens.Count == 0)
        {
            Engine.Close();
        }
    }

    protected override void RenderScene(CommandList commandList)
    {
        CameraData cameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        RenderArgs args = new RenderArgs(commandList, LayerType.Gui, MatrixStack, cameraData);
        
        ScreenHandler.Render(args);
        
        MatrixStack.Push();
        MatrixStack.Push();
        MatrixStack.Translate(0.5f, 0.5f, 0.0f);
        MatrixStack.Scale(Engine.ClientSize.X, 256.0f, 1.0f);
        EntityManager.Invoke(border, entity => entity.Render(args));
        MatrixStack.Pop();
        MatrixStack.Translate(32.0f, 32.0f, 0.0f);
        EntityManager.Invoke(bannerEntity, entity => entity.Render(args));
        MatrixStack.Pop();
        
        MatrixStack.Push();
        MatrixStack.Translate(0.0f, Engine.ClientSize.Y - 64.0f, 0.0f);
        MatrixStack.Push();
        MatrixStack.Translate(0.5f, 0.5f, 0.0f);
        MatrixStack.Scale(Engine.ClientSize.X, 64.0f, 1.0f);
        EntityManager.Invoke(border, entity => entity.Render(args));
        MatrixStack.Pop();
        MatrixStack.Translate(Engine.ClientSize.X - 16.0f, 24.0f, 0.0f);
        EntityManager.Invoke(copyrightText, entity => entity.Render(args));
        MatrixStack.Pop();
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
    }
}