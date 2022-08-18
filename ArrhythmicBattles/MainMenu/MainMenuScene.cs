using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework.Core;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering.Data;
using OpenTK.Mathematics;
using Textwriter;
using Renderer = FlexFramework.Rendering.Renderer;

namespace ArrhythmicBattles.MainMenu;

public class MainMenuScene : Scene
{
    private int guiLayer;
    private MatrixStack transform = new MatrixStack();

    private Texture2D bannerTexture;
    private ImageEntity bannerEntity;
    
    private MeshEntity header;
    private MeshEntity footer;
    
    private TextEntity copyrightText;

    private Buttons buttons;

    private GuiCamera guiCamera;

    public override void Init()
    {
        Renderer renderer = Engine.Renderer;

        renderer.ClearColor = new Color4(33, 33, 33, 255);
        guiLayer = renderer.GetLayerId("gui");

        guiCamera = new GuiCamera(Engine);
        
        bannerTexture = Texture2D.FromFile("banner", "Assets/banner.png");
        bannerEntity = new ImageEntity(Engine)
            .WithPosition(32, 32)
            .WithSize(0, 192)
            .WithTexture(bannerTexture)
            .WithImageMode(ImageMode.Stretch);

        header = new MeshEntity();
        header.Color = new Color4(24, 24, 24, 255);
        header.Mesh = Engine.PersistentResources.QuadMesh;
        
        footer = new MeshEntity();
        footer.Color = new Color4(24, 24, 24, 255);
        footer.Mesh = Engine.PersistentResources.QuadMesh;

        copyrightText = new TextEntity(Engine, Engine.TextResources.GetFont("inconsolata-small"));
        copyrightText.HorizontalAlignment = HorizontalAlignment.Right;
        copyrightText.Text = "Copyright Arrhythmic Battles 2022\nThis project is Free Software under the GPLv3";

        buttons = new Buttons(Engine);
    }

    public override void Update(UpdateArgs args)
    {
        header.Update(args);
        footer.Update(args);
        bannerEntity.Update(args);
        copyrightText.Update(args);
        
        buttons.Update(args);
    }

    public override void Render(Renderer renderer)
    {
        CameraData cameraData = guiCamera.GetCameraData(Engine.ClientSize);
        
        transform.Push();
        transform.Translate(0.5, 0.5, 0.0);
        transform.Scale(Engine.ClientSize.X, 256.0, 1.0);
        header.Render(renderer, guiLayer, transform, cameraData);
        transform.Pop();
        
        transform.Push();
        transform.Translate(0.5, 0.5, 0.0);
        transform.Scale(Engine.ClientSize.X, 64.0, 1.0);
        transform.Translate(0.0, Engine.ClientSize.Y - 64.0, 0.0);
        footer.Render(renderer, guiLayer, transform, cameraData);
        transform.Pop();

        bannerEntity.Render(renderer, guiLayer, transform, cameraData);
        
        transform.Push();
        transform.Translate(Engine.ClientSize.X - 16.0, Engine.ClientSize.Y - 36.0, 0.0);
        copyrightText.Render(renderer, guiLayer, transform, cameraData);
        transform.Pop();
        
        buttons.Render(renderer, guiLayer, transform, cameraData);
    }

    public override void Dispose()
    {
        bannerEntity.Dispose();
        buttons.Dispose();
    }
}