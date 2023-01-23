using System.Collections;
using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;
using Textwriter;
using Renderer = FlexFramework.Core.Rendering.Renderer;

namespace ArrhythmicBattles.MainMenu;

public class MainMenuScene : ABScene
{
    private Texture2D bannerTexture;
    private ImageEntity bannerEntity;
    
    private MeshEntity header;
    private MeshEntity footer;
    
    private TextEntity copyrightText;

    private InputInfo inputInfo;

    public MainMenuScene(ABContext context) : base(context)
    {
    }

    public override void Init()
    {
        base.Init();
        
        // Reset lightings
        if (Engine.Renderer is ILighting lightings)
        {
            lightings.DirectionalLight = null;
        }
        
        // Init audio
        Context.Sound.MenuBackgroundMusic.Play();

        // Init entities
        bannerTexture = Texture2D.FromFile("banner", "Assets/banner.png");
        bannerEntity = new ImageEntity(Engine);
        bannerEntity.Position = new Vector2(32.0f, 32.0f);
        bannerEntity.Size = new Vector2(0.0f, 192.0f);
        bannerEntity.Texture = bannerTexture;
        bannerEntity.ImageMode = ImageMode.Stretch;

        header = new MeshEntity();
        header.Color = new Color4(24, 24, 24, 255);
        header.Mesh = Engine.PersistentResources.QuadMesh;
        
        footer = new MeshEntity();
        footer.Color = new Color4(24, 24, 24, 255);
        footer.Mesh = Engine.PersistentResources.QuadMesh;

        copyrightText = new TextEntity(Engine, Engine.TextResources.GetFont("inconsolata-small"));
        copyrightText.HorizontalAlignment = HorizontalAlignment.Right;
        // copyrightText.Text = "Copyright Arrhythmic Battles 2022\nThis project is Free Software under the GPLv3";
        copyrightText.Text = "Luce, do not.\nLuce, your status.";
        
        // Init input
        inputInfo = Context.InputSystem.GetInputInfo();
        
        OpenScreen(new SelectScreen(Engine, this, inputInfo));
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
        
        MatrixStack.Push();
        MatrixStack.Translate(48.0f, 306.0f, 0.0f);
        ScreenHandler.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();
        
        MatrixStack.Push();
        MatrixStack.Push();
        MatrixStack.Translate(0.5f, 0.5f, 0.0f);
        MatrixStack.Scale(Engine.ClientSize.X, 256.0f, 1.0f);
        header.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();
        bannerEntity.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();
        
        MatrixStack.Push();
        MatrixStack.Translate(0.0f, Engine.ClientSize.Y - 64.0f, 0.0f);
        MatrixStack.Push();
        MatrixStack.Translate(0.5f, 0.5f, 0.0f);
        MatrixStack.Scale(Engine.ClientSize.X, 64.0f, 1.0f);
        footer.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();
        MatrixStack.Translate(Engine.ClientSize.X - 16.0f, 24.0f, 0.0f);
        copyrightText.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Context.Sound.MenuBackgroundMusic.Stop();
    }
}