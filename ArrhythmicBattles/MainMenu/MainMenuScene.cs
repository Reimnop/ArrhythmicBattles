using System.Collections;
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

public class MainMenuScene : GuiScene
{
    public struct MenuItemsOffset
    {
        public double ScreenXOffset { get; set; }
        public double HeaderYOffset { get; set; }
        public double FooterYOffset { get; set; }

        public MenuItemsOffset(double screenX, double headerY, double footerY)
        {
            ScreenXOffset = screenX;
            HeaderYOffset = headerY;
            FooterYOffset = footerY;
        }

        public static MenuItemsOffset Lerp(MenuItemsOffset left, MenuItemsOffset right, double factor)
        {
            return new MenuItemsOffset(
                MathHelper.Lerp(left.ScreenXOffset, right.ScreenXOffset, factor),
                MathHelper.Lerp(left.HeaderYOffset, right.HeaderYOffset, factor),
                MathHelper.Lerp(left.FooterYOffset, right.FooterYOffset, factor));
        }
    }

    public ABContext Context { get; }

    private Texture2D bannerTexture;
    private ImageEntity bannerEntity;
    
    private MeshEntity header;
    private MeshEntity footer;
    
    private TextEntity copyrightText;

    private MenuItemsOffset menuItemsOffset = new MenuItemsOffset(-656.0, -256.0, 64.0);
    private double deltaTime;

    private SimpleAnimator<MenuItemsOffset> menuAnimator;
    private Screen currentScreen;
    private double screenYOffset = 0.0;

    public MainMenuScene(ABContext context)
    {
        Context = context;
    }

    public override void Init()
    {
        base.Init();
        
        // Init audio
        Context.Sound.MenuBackgroundMusic.Play();

        // Init entities
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
        
        // Init screen
        currentScreen = new SelectScreen(Engine, this);

        menuAnimator = new SimpleAnimator<MenuItemsOffset>(
            (left, right, factor) =>
            {
                double t = Easing.QuadInOut(factor);
                return MenuItemsOffset.Lerp(left, right, Easing.CircInOut(t));
            },
            value => menuItemsOffset = value, 
            () => new MenuItemsOffset(-Engine.ClientSize.X, -256.0, 64.0),
            2.5);
        
        // Init startup sequence
        StartCoroutine(ShowMenu());
    }

    public void SwitchScreen<T>(params object?[]? args) where T : Screen
    {
        currentScreen.Dispose();
        
        Screen? screen = (Screen?) Activator.CreateInstance(typeof(T), args);
        if (screen == null)
        {
            throw new Exception("Could not load screen!");
        }

        currentScreen = screen;

        StartCoroutine(AnimateSwitchScreen(screen));
    }

    private IEnumerator AnimateSwitchScreen(Screen screen)
    {
        for (double t = 0.0; t < 1.0; t += deltaTime * 10.0)
        {
            screenYOffset = Math.Sin(t * Math.PI) * 8.0;
            yield return null;
        }

        screenYOffset = 0.0;
    }

    public void LoadScene<T>(params object?[]? args) where T : Scene
    {
        StartCoroutine(HideMenuAndDo(() => Engine.LoadScene<T>(args)));
    }

    private IEnumerator HideMenuAndDo(Action action)
    {
        yield return HideMenu();
        action();
    }

    private IEnumerator ShowMenu()
    {
        // for some reason it won't work without waiting for a frame
        yield return null;
        menuAnimator.LerpTo(() => new MenuItemsOffset());
        yield return menuAnimator.WaitUntilFinish();
    }

    private IEnumerator HideMenu()
    {
        menuAnimator.LerpTo(() => new MenuItemsOffset(-656.0, -256.0, 64.0));
        yield return menuAnimator.WaitUntilFinish();
    }

    public override void Update(UpdateArgs args)
    {
        deltaTime = args.DeltaTime;
        
        header.Update(args);
        footer.Update(args);
        bannerEntity.Update(args);
        copyrightText.Update(args);
        Context.Update();
        
        menuAnimator.Update(args.DeltaTime);

        currentScreen.Position = new Vector2i(48 + (int) menuItemsOffset.ScreenXOffset, 306);
        currentScreen.Update(args);
    }

    public override void Render(Renderer renderer)
    {
        CameraData cameraData = Camera.GetCameraData(Engine.ClientSize);
        
        MatrixStack.Push();
        MatrixStack.Translate(0.0, screenYOffset, 0.0);
        currentScreen.Render(renderer, GuiLayerId, MatrixStack, cameraData);
        MatrixStack.Pop();
        
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
    }

    public override void Dispose()
    {
        Context.Sound.MenuBackgroundMusic.Stop();
        
        bannerEntity.Dispose();
        currentScreen.Dispose();
    }
}