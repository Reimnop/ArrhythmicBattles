using ArrhythmicBattles.Game;
using ArrhythmicBattles.Menu;
using ArrhythmicBattles.UserInterface.Animation;
using FlexFramework;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using FlexFramework.Logging;
using Glide;
using OpenTK.Windowing.Common.Input;
using StbImageSharp;
using Image = OpenTK.Windowing.Common.Input.Image;

namespace ArrhythmicBattles;

public class Program
{
    public static void Main(string[] args)
    {
        // Init glide
        Tweener.SetLerper<Vector2Lerper>(typeof(Vector2));
        Tweener.SetLerper<Color4Lerper>(typeof(Color4));
        
        Image imageS = GetImageFromFile("Assets/Icons/icon_s.png");
        Image imageM = GetImageFromFile("Assets/Icons/icon_m.png");
        Image imageL = GetImageFromFile("Assets/Icons/icon_l.png");
        Image imageXl = GetImageFromFile("Assets/Icons/icon_xl.png");

        WindowIcon icon = new WindowIcon(imageS, imageM, imageL, imageXl);

        NativeWindowSettings nws = new NativeWindowSettings()
        {
            Title = "Arrhythmic Battles",
            Size = new Vector2i(1366, 768),
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 3),
            AutoLoadBindings = true,
            Profile = ContextProfile.Core,
            Icon = icon
        };

        using FlexFrameworkMain flexFramework = new FlexFrameworkMain(nws);
        flexFramework.Log += OnLog;
        flexFramework.VSync = VSyncMode.On;

        using ABContext context = new ABContext(flexFramework);

        flexFramework.UseRenderer(new DefaultRenderer());
        flexFramework.LoadFonts(2048,
            new FontFileInfo("inconsolata-regular", 24, "Assets/Fonts/Inconsolata-Regular.ttf"), 
            new FontFileInfo("inconsolata-small", 16, "Assets/Fonts/Inconsolata-Regular.ttf"));
        
#if DEBUG_SKIP_MENU
        flexFramework.LoadScene(new GameScene(context));
#else
        flexFramework.LoadScene(new MainMenuScene(context));
#endif

        while (!flexFramework.ShouldClose())
        {
            context.Update();
            flexFramework.Update();
        }
    }
    
    private static void OnLog(object sender, LogEventArgs eventArgs)
    {
        switch (eventArgs.Severity)
        {
            case Severity.Debug:
                Log("DEBUG", $"[FlexFramework] {eventArgs.Message}");
                break;
            case Severity.Info:
                Log("INFO", $"[FlexFramework] {eventArgs.Message}");
                break;
            case Severity.Warning:
                Log("WARN", $"[FlexFramework] {eventArgs.Message}");
                break;
            case Severity.Error:
                Log("ERROR", $"[FlexFramework] {eventArgs.Message}");
                break;
            case Severity.Fatal:
                Log("FATAL", $"[FlexFramework] {eventArgs.Message}");
                break;
        }
    }
    
    private static void Log(string severity, string message)
    {
        Console.WriteLine($"{DateTime.Now:hh:mm:ss} | {severity} | {message}");
    }
    
    private static Image GetImageFromFile(string path)
    {
        using FileStream stream = File.OpenRead(path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        return new Image(result.Width, result.Height, result.Data);
    }
}