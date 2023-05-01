using ArrhythmicBattles.UserInterface;
using ArrhythmicBattles.Util;
using ArrhythmicBattles.Core;
using FlexFramework.Core;
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
    private ImageEntity bannerEntity = null!;
    private TextEntity copyrightText = null!;

    private MeshEntity border = null!;
    
    private ScopedInputProvider inputProvider = null!;
    private Texture2D bannerTexture = null!;

    public MainMenuScene(ABContext context) : base(context)
    {
    }

    public override void Init()
    {
        base.Init();
        
        Engine.CursorState = CursorState.Normal;
        
        // Reset lightings
        if (Engine.Renderer is ILighting lightings)
        {
            lightings.DirectionalLight = null;
        }
        
        // Init audio
        Context.Sound.MenuBackgroundMusic.Play();

        // Init entities
        string bannerPath = RandomHelper.RandomFromTime() < 0.002 ? "Assets/banner_alt.png" : "Assets/banner.png"; // Sneaky easter egg
        
        bannerTexture = Texture2D.FromFile("banner", bannerPath);
        RegisterObject(bannerTexture);
        
        bannerEntity = new ImageEntity(Engine);
        bannerEntity.Position = new Vector2(32.0f, 32.0f);
        bannerEntity.Size = new Vector2(0.0f, 192.0f);
        bannerEntity.Texture = bannerTexture;
        bannerEntity.ImageMode = ImageMode.Stretch;
        RegisterObject(bannerEntity);

        var textAssetsLocation = Engine.DefaultAssets.TextAssets;
        var textAssets = Engine.ResourceRegistry.GetResource(textAssetsLocation);
        Font font = textAssets[Constants.DefaultFontName];
        
        copyrightText = new TextEntity(Engine, font);
        copyrightText.EmSize = 18.0f / 24.0f;
        copyrightText.HorizontalAlignment = HorizontalAlignment.Right;
        copyrightText.Text = $"Version 0.0.1 BETA\n© {DateTime.Now.Year} Arrhythmic Battles";
        // copyrightText.Text = "Luce, do not.\nLuce, your status.";
        RegisterObject(copyrightText);
        
        EngineAssets assets = Engine.DefaultAssets;
        Mesh<Vertex> quadMesh = Engine.ResourceRegistry.GetResource(assets.QuadMesh);

        border = new MeshEntity();
        border.Color = new Color4(24, 24, 24, 255);
        border.Mesh = quadMesh;
        RegisterObject(border);

        // Init input
        inputProvider = Context.InputSystem.AcquireInputProvider();
        RegisterObject(inputProvider);
        
        // Init UI
        ScreenBounds = new Bounds(48.0f, 306.0f, 816.0f, 0.0f);
        OpenScreen(new SelectScreen(Engine, this, inputProvider));
    }

    public override void SwitchScreen(Screen before, Screen after)
    {
        base.SwitchScreen(before, after);
        
        Context.Sound.SelectSfx.Play();
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
        CameraData cameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        RenderArgs args = new RenderArgs(renderer, GuiLayerId, MatrixStack, cameraData);
        
        ScreenHandler.Render(args);
        
        MatrixStack.Push();
        MatrixStack.Push();
        MatrixStack.Translate(0.5f, 0.5f, 0.0f);
        MatrixStack.Scale(Engine.ClientSize.X, 256.0f, 1.0f);
        border.Render(args);
        MatrixStack.Pop();
        bannerEntity.Render(args);
        MatrixStack.Pop();
        
        MatrixStack.Push();
        MatrixStack.Translate(0.0f, Engine.ClientSize.Y - 64.0f, 0.0f);
        MatrixStack.Push();
        MatrixStack.Translate(0.5f, 0.5f, 0.0f);
        MatrixStack.Scale(Engine.ClientSize.X, 64.0f, 1.0f);
        border.Render(args);
        MatrixStack.Pop();
        MatrixStack.Translate(Engine.ClientSize.X - 16.0f, 24.0f, 0.0f);
        copyrightText.Render(args);
        MatrixStack.Pop();
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Context.Sound.MenuBackgroundMusic.Stop();
    }
}