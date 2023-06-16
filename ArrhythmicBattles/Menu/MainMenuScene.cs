﻿using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using ArrhythmicBattles.Core;
using ArrhythmicBattles.Settings;
using FlexFramework.Core;
using FlexFramework.Core.Audio;
using FlexFramework.Core.Data;
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
            AudioStream = resourceManager.Load<AudioStream>("Audio/Arrhythmic.ogg")
        };
        musicAudioSource.Play();

        sfxAudioSource = new AudioSource
        {
            Looping = false,
            AudioStream = resourceManager.Load<AudioStream>("Audio/Select.ogg")
        };

        // Init bindings
        musicVolumeBinding = new Binding<float>(settings, nameof(ISettings.MusicVolume), musicAudioSource, nameof(AudioSource.Gain));
        sfxVolumeBinding = new Binding<float>(settings, nameof(ISettings.SfxVolume), sfxAudioSource, nameof(AudioSource.Gain));
        
        // Init input
        inputProvider = Context.InputSystem.AcquireInputProvider();

        // Init UI
        var backgroundTexture = resourceManager.Load<TextureSampler>("Textures/Background.png");
        var bannerTexture = resourceManager.Load<TextureSampler>("Textures/Banner.png");
        var boldFont = resourceManager.Load<Font>(Constants.BoldFontPath);
        
        // Magic numbers retrieved from design in Figma
        screenManager = new ScreenManager(currentScreenBounds, child => 
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder() // Background
                    .SetElement(new MouseAffectedImageElement(inputProvider, backgroundTexture))
                    .SetAnchor(Anchor.Fill)) 
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
        var guiCameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        var guiArgs = new RenderArgs(commandList, LayerType.Gui, MatrixStack, guiCameraData);
        screenManager.Render(guiArgs);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        musicVolumeBinding.Dispose();
        sfxVolumeBinding.Dispose();
        inputProvider.Dispose();
        musicAudioSource.Dispose();
        sfxAudioSource.Dispose();
        screenManager.Dispose();
    }
}