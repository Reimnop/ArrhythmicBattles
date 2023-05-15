using ArrhythmicBattles.Core.Animation;
using ArrhythmicBattles.Menu;
using FlexFramework;
using FlexFramework.Core.Rendering.Renderers;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using FlexFramework.Logging;
using FlexFramework.Modelling.Animation;
using Glide;
using OpenTK.Windowing.Common.Input;
using SixLabors.ImageSharp.PixelFormats;

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
        flexFramework.UseRenderer(new DefaultRenderer());

        using ABContext context = new ABContext(flexFramework);

        var textAssetsLocation = flexFramework.DefaultAssets.TextAssets;
        var textAssets = flexFramework.ResourceRegistry.GetResource(textAssetsLocation);
        textAssets.LoadFont("Assets/Fonts/Inconsolata-Regular.ttf", Constants.DefaultFontName, 24);

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
        using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(path);
        byte[] pixels = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(pixels);
        
        return new Image(image.Width, image.Height, pixels);
    }
}