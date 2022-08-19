using System.Collections;
using ArrhythmicBattles.Settings;
using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using DiscordRPC;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering.Data;
using OpenTK.Mathematics;
using Textwriter;
using Renderer = FlexFramework.Rendering.Renderer;

namespace ArrhythmicBattles.MainMenu;

public class MainMenuScene : GuiScene
{
    public struct MenuItemsOffset
    {
        public double ButtonsXOffset { get; set; }
        public double HeaderYOffset { get; set; }
        public double FooterYOffset { get; set; }

        public MenuItemsOffset(double buttonX, double headerY, double footerY)
        {
            ButtonsXOffset = buttonX;
            HeaderYOffset = headerY;
            FooterYOffset = footerY;
        }

        public static MenuItemsOffset Lerp(MenuItemsOffset left, MenuItemsOffset right, double factor)
        {
            return new MenuItemsOffset(
                MathHelper.Lerp(left.ButtonsXOffset, right.ButtonsXOffset, factor),
                MathHelper.Lerp(left.HeaderYOffset, right.HeaderYOffset, factor),
                MathHelper.Lerp(left.FooterYOffset, right.FooterYOffset, factor));
        }
    }
    
    private readonly ABContext context;
    
    private Texture2D bannerTexture;
    private ImageEntity bannerEntity;
    
    private MeshEntity header;
    private MeshEntity footer;
    
    private TextEntity copyrightText;

    private Buttons buttons;

    private MenuItemsOffset menuItemsOffset = new MenuItemsOffset(-656.0, -256.0, 64.0);
    private double deltaTime;

    public MainMenuScene(ABContext context)
    {
        this.context = context;
    }

    public override void Init()
    {
        base.Init();
        
        // Init other stuff
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
        // copyrightText.Text = "Copyright Arrhythmic Battles 2022\nThis project is Free Software under the GPLv3";
        copyrightText.Text = "Luce, do not.\nLuce, your status.";
        
        buttons = new Buttons(Engine, this, new Vector2i(512, 56));
        
        StartCoroutine(ShowMenu());

        // Set Discord presence
        context.DiscordRpcClient.SetPresence(new RichPresence
        {
            Details = "In Main Menu",
            State = "Idle",
            Timestamps = new Timestamps(DateTime.UtcNow),
            Assets = new Assets
            {
                LargeImageKey = "ab_logo",
                LargeImageText = "Arrhythmic Battles"
            }
        });
    }

    public void LoadSettingsScene()
    {
        StartCoroutine(HideMenuAndDo(() => Engine.LoadScene<SettingsScene>(context)));
    }

    private IEnumerator HideMenuAndDo(Action action)
    {
        yield return HideMenu();
        action();
    }

    private IEnumerator ShowMenu()
    {
        yield return null;
        
        double t = 0.0;
        while (t < 1.0)
        {
            t += deltaTime * 2.5;
            menuItemsOffset = MenuItemsOffset.Lerp(
                new MenuItemsOffset(-656.0, -256.0, 64.0),
                new MenuItemsOffset(),
                Easing.InOutCirc(t));
            yield return WaitForEndOfFrame();
        }

        menuItemsOffset = new MenuItemsOffset();
    }

    private IEnumerator HideMenu()
    {
        double t = 0.0;
        while (t < 1.0)
        {
            t += deltaTime * 2.5;
            menuItemsOffset = MenuItemsOffset.Lerp(
                new MenuItemsOffset(),
                new MenuItemsOffset(-656.0, -256.0, 64.0),
                Easing.InOutCirc(t));
            yield return WaitForEndOfFrame();
        }

        menuItemsOffset = new MenuItemsOffset(-656.0, -256.0, 64.0);
    }

    public override void Update(UpdateArgs args)
    {
        deltaTime = args.DeltaTime;
        
        header.Update(args);
        footer.Update(args);
        bannerEntity.Update(args);
        copyrightText.Update(args);
        context.Update();

        buttons.Position = new Vector2i(48 + (int) menuItemsOffset.ButtonsXOffset, 306);
        buttons.Update(args);
    }

    public override void Render(Renderer renderer)
    {
        CameraData cameraData = Camera.GetCameraData(Engine.ClientSize);

        MatrixStack.Push();
        MatrixStack.Translate(0.0, menuItemsOffset.HeaderYOffset, 0.0);
        MatrixStack.Push();
        MatrixStack.Translate(0.5, 0.5, 0.0);
        MatrixStack.Scale(Engine.ClientSize.X, 256.0, 1.0);
        header.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();
        bannerEntity.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();
        
        MatrixStack.Push();
        MatrixStack.Translate(0.0, Engine.ClientSize.Y - 64.0, 0.0);
        MatrixStack.Translate(0.0, menuItemsOffset.FooterYOffset, 0.0);
        MatrixStack.Push();
        MatrixStack.Translate(0.5, 0.5, 0.0);
        MatrixStack.Scale(Engine.ClientSize.X, 64.0, 1.0);
        footer.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();
        MatrixStack.Translate(Engine.ClientSize.X - 16.0, 24.0, 0.0);
        copyrightText.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();

        buttons.Render(renderer, GuiLayerId, MatrixStack, cameraData);
    }

    public override void Dispose()
    {
        bannerEntity.Dispose();
        buttons.Dispose();
    }
}