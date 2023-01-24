using ArrhythmicBattles.Game;
using FlexFramework;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using ArrhythmicBattles.Menu;
using DiscordRPC.Logging;
using FlexFramework.Logging;
using log4net;
using log4net.Config;
using OpenTK.Windowing.Common.Input;
using StbImageSharp;

namespace ArrhythmicBattles;

public class Program
{
    private static ILog log = LogManager.GetLogger("ArrhythmicBattles");

    public static void Main(string[] args)
    {
        BasicConfigurator.Configure();
        
        Image imageS = GetImageFromFile("Assets/Icons/icon_s.png");
        Image imageM = GetImageFromFile("Assets/Icons/icon_m.png");
        Image imageL = GetImageFromFile("Assets/Icons/icon_l.png");
        Image imageXl = GetImageFromFile("Assets/Icons/icon_xl.png");

        WindowIcon icon = new WindowIcon(imageS, imageM, imageL, imageXl);

        NativeWindowSettings nws = new NativeWindowSettings()
        {
            Title = "Arrhythmic Battles",
            Size = new Vector2i(1366, 768),
            APIVersion = new Version(4, 3),
            Profile = ContextProfile.Core,
            Icon = icon
        };

        using FlexFrameworkMain flexFramework = new FlexFrameworkMain(nws);
        flexFramework.Log += OnLog;
        flexFramework.CursorState = CursorState.Grabbed;

        using ABContext context = new ABContext(flexFramework);

        flexFramework.UseRenderer<DefaultRenderer>();
        flexFramework.LoadFonts(2048,
            new FontFileInfo("inconsolata-regular", 24, "Assets/Fonts/Inconsolata-Regular.ttf"), 
            new FontFileInfo("inconsolata-small", 16, "Assets/Fonts/Inconsolata-Regular.ttf"));
        
#if DEBUG
        flexFramework.LoadScene(new GameScene(context));
        flexFramework.VSync = VSyncMode.Off;
#else
        flexFramework.LoadScene(new MainMenuScene(context));
#endif

        while (!flexFramework.ShouldClose())
        {
            flexFramework.Update();
        }
    }
    
    private static void OnLog(object sender, LogEventArgs eventArgs)
    {
        switch (eventArgs.Severity)
        {
            case Severity.Debug:
                log.Debug($"[FlexFramework] {eventArgs.Message}");
                break;
            case Severity.Info:
                log.Info($"[FlexFramework] {eventArgs.Message}");
                break;
            case Severity.Warning:
                log.Warn($"[FlexFramework] {eventArgs.Message}");
                break;
            case Severity.Error:
                log.Error($"[FlexFramework] {eventArgs.Message}");
                break;
            case Severity.Fatal:
                log.Fatal($"[FlexFramework] {eventArgs.Message}");
                break;
        }
    }
    
    private static Image GetImageFromFile(string path)
    {
        using FileStream stream = File.OpenRead(path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        return new Image(result.Width, result.Height, result.Data);
    }
}