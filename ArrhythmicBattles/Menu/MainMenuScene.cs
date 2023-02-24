using System.Collections;
using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Textwriter;
using Renderer = FlexFramework.Core.Rendering.Renderer;

namespace ArrhythmicBattles.Menu;

public class MainMenuScene : ABScene
{
    private Texture2D bannerTexture;
    private ImageEntity bannerEntity;
    
    private MeshEntity header;
    private MeshEntity footer;
    
    private TextEntity copyrightText;

    private ScopedInputProvider inputProvider;

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
        string bannerPath = Utils.RandomFromTime() < 0.002 ? "Assets/banner_alt.png" : "Assets/banner.png"; // Sneaky easter egg
        bannerTexture = Texture2D.FromFile("banner", bannerPath);
        bannerEntity = new ImageEntity(Engine);
        bannerEntity.Position = new Vector2(32.0f, 32.0f);
        bannerEntity.Size = new Vector2(0.0f, 192.0f);
        bannerEntity.Texture = bannerTexture;
        bannerEntity.ImageMode = ImageMode.Stretch;
        
        EngineResources resources = Engine.Resources;

        header = new MeshEntity();
        header.Color = new Color4(24, 24, 24, 255);
        header.Mesh = Engine.ResourceManager.GetResource<Mesh<Vertex>>(resources.QuadMesh);;
        
        footer = new MeshEntity();
        footer.Color = new Color4(24, 24, 24, 255);
        footer.Mesh = Engine.ResourceManager.GetResource<Mesh<Vertex>>(resources.QuadMesh);;

        copyrightText = new TextEntity(Engine, Engine.TextResources.GetFont("inconsolata-small"));
        copyrightText.HorizontalAlignment = HorizontalAlignment.Right;
        copyrightText.Text = "Version 0.0.1 BETA\n© 2021 Arrhythmic Battles"; // TODO: It's not 2021 anymore
        // copyrightText.Text = "Luce, do not.\nLuce, your status.";
        
        // Init input
        inputProvider = Context.InputSystem.AcquireInputProvider();
        
        OpenScreen(new SelectScreen(Engine, this, inputProvider));
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);

        header.Update(args);
        footer.Update(args);
        bannerEntity.Update(args);
        copyrightText.Update(args);
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
        header.Render(args);
        MatrixStack.Pop();
        bannerEntity.Render(args);
        MatrixStack.Pop();
        
        MatrixStack.Push();
        MatrixStack.Translate(0.0f, Engine.ClientSize.Y - 64.0f, 0.0f);
        MatrixStack.Push();
        MatrixStack.Translate(0.5f, 0.5f, 0.0f);
        MatrixStack.Scale(Engine.ClientSize.X, 64.0f, 1.0f);
        footer.Render(args);
        MatrixStack.Pop();
        MatrixStack.Translate(Engine.ClientSize.X - 16.0f, 24.0f, 0.0f);
        copyrightText.Render(args);
        MatrixStack.Pop();
    }

    public override void Dispose()
    {
        base.Dispose();
        
        inputProvider.Dispose();
        Context.Sound.MenuBackgroundMusic.Stop();
    }
}