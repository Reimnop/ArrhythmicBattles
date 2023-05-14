﻿using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using ArrhythmicBattles.Core;
using ArrhythmicBattles.Settings;
using FlexFramework.Core;
using FlexFramework.Core.Audio;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Textwriter;
using Renderer = FlexFramework.Core.Rendering.Renderer;

namespace ArrhythmicBattles.Menu;

public class MainMenuScene : ABScene
{
    // Resources
    private readonly Texture bannerTexture;
    
    // Entities
    private readonly ImageEntity bannerEntity;
    private readonly TextEntity copyrightText;
    private readonly MeshEntity border;
    
    // Other things
    private readonly AudioStream menuAudioStream;
    private readonly AudioSource menuAudioSource;
    private readonly AudioStream sfxAudioStream;
    private readonly AudioSource sfxAudioSource;
    private readonly ScopedInputProvider inputProvider;

    private readonly CommandList commandList = new();

    public MainMenuScene(ABContext context) : base(context)
    {
        Engine.CursorState = CursorState.Normal;

        // Reset lightings
        if (Engine.Renderer is ILighting lightings)
        {
            lightings.DirectionalLight = null;
        }
        
        // Init audio
        // TODO: Make it less imperative
        var settings = Context.Settings;
        
        menuAudioStream = new VorbisAudioStream("Assets/Audio/Arrhythmic.ogg");
        menuAudioSource = new AudioSource();
        menuAudioSource.Gain = settings.MusicVolume;
        menuAudioSource.AudioStream = menuAudioStream;
        menuAudioSource.Play();

        sfxAudioStream = new VorbisAudioStream("Assets/Audio/Select.ogg");
        sfxAudioSource = new AudioSource();
        sfxAudioSource.Gain = settings.SfxVolume;
        sfxAudioSource.Looping = false;
        sfxAudioSource.AudioStream = sfxAudioStream;
        
        settings.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(ISettings.MusicVolume))
            {
                menuAudioSource.Gain = settings.MusicVolume;
            }

            if (args.PropertyName == nameof(ISettings.SfxVolume))
            {
                sfxAudioSource.Gain = settings.SfxVolume;
            }
        };

        // Init resources
        string bannerPath = RandomHelper.RandomFromTime() < 0.002 ? "Assets/banner_alt.png" : "Assets/banner.png"; // Sneaky easter egg
        bannerTexture = Texture.FromFile("banner", bannerPath);
        
        var assets = Engine.DefaultAssets;
        var quadMesh = Engine.ResourceRegistry.GetResource(assets.QuadMesh);

        // Init entities
        bannerEntity = CreateEntity(() => new ImageEntity(Engine));
        bannerEntity.Position = new Vector2(32.0f, 32.0f);
        bannerEntity.Size = new Vector2(0.0f, 192.0f);
        bannerEntity.Texture = bannerTexture;
        bannerEntity.ImageMode = ImageMode.Stretch;

        var textAssetsLocation = Engine.DefaultAssets.TextAssets;
        var textAssets = Engine.ResourceRegistry.GetResource(textAssetsLocation);
        Font font = textAssets[Constants.DefaultFontName];
        
        copyrightText = CreateEntity(() => new TextEntity(Engine, font));
        copyrightText.EmSize = 18.0f / 24.0f;
        copyrightText.HorizontalAlignment = HorizontalAlignment.Right;
        copyrightText.Text = $"Version 0.0.1 BETA\n© {DateTime.Now.Year} Arrhythmic Battles";
        // copyrightText.Text = "Luce, do not.\nLuce, your status.";

        border = CreateEntity(() => new MeshEntity());
        border.Color = new Color4(24, 24, 24, 255);
        border.Mesh = quadMesh;

        // Init input
        inputProvider = Context.InputSystem.AcquireInputProvider();

        // Init UI
        ScreenBounds = new Bounds(48.0f, 306.0f, 816.0f, 0.0f);
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

    public override void Render(Renderer renderer)
    {
        commandList.Clear();
        
        CameraData cameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        RenderArgs args = new RenderArgs(commandList, LayerType.Gui, MatrixStack, cameraData);
        
        ScreenHandler.Render(args);
        
        MatrixStack.Push();
        MatrixStack.Push();
        MatrixStack.Translate(0.5f, 0.5f, 0.0f);
        MatrixStack.Scale(Engine.ClientSize.X, 256.0f, 1.0f);
        EntityCall(border, entity => entity.Render(args));
        MatrixStack.Pop();
        EntityCall(bannerEntity, entity => entity.Render(args));
        MatrixStack.Pop();
        
        MatrixStack.Push();
        MatrixStack.Translate(0.0f, Engine.ClientSize.Y - 64.0f, 0.0f);
        MatrixStack.Push();
        MatrixStack.Translate(0.5f, 0.5f, 0.0f);
        MatrixStack.Scale(Engine.ClientSize.X, 64.0f, 1.0f);
        EntityCall(border, entity => entity.Render(args));
        MatrixStack.Pop();
        MatrixStack.Translate(Engine.ClientSize.X - 16.0f, 24.0f, 0.0f);
        EntityCall(copyrightText, entity => entity.Render(args));
        MatrixStack.Pop();
        
        // Render scene
        renderer.Render(Engine.ClientSize, commandList, Context.RenderBuffer);
        Engine.Present(Context.RenderBuffer);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        inputProvider.Dispose();
        // Context.Sound.MenuBackgroundMusic.Stop();
        menuAudioSource.Dispose();
        menuAudioStream.Dispose();
        sfxAudioSource.Dispose();
        sfxAudioStream.Dispose();
    }
}