using ArrhythmicBattles.Core.Animation;
using ArrhythmicBattles.Core.IO;
using ArrhythmicBattles.Core.Resource;
using ArrhythmicBattles.Game;
using ArrhythmicBattles.Intro;
using FlexFramework;
using FlexFramework.Core.Rendering.Renderers;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using FlexFramework.Util.Logging;
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
        Tweener.SetLerper<Box2Lerper>(typeof(Box2));
        Tweener.SetLerper<Color4Lerper>(typeof(Color4));
        Tweener.SetLerper<Vector2Lerper>(typeof(Vector2));
        
        // Init resources
        var fileSystem = new RelativeFileSystem(Constants.GlobalResourcesPath);
        var resourceManager = new ResourceManager(fileSystem);

        var imageS = resourceManager.Load<Image>("Icons/icon_s.png");
        var imageM = resourceManager.Load<Image>("Icons/icon_m.png");
        var imageL = resourceManager.Load<Image>("Icons/icon_l.png");
        var imageXl = resourceManager.Load<Image>("Icons/icon_xl.png");
        var icon = new WindowIcon(imageS, imageM, imageL, imageXl);

        var nws = new NativeWindowSettings()
        {
            Title = Constants.GameName,
            Size = new Vector2i(1366, 768),
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 3),
            AutoLoadBindings = true,
            Profile = ContextProfile.Core,
            Icon = icon
        };

        using var flexFramework = new FlexFrameworkMain(nws, engine => new DefaultRenderer(engine), OnLog);
        flexFramework.VSync = VSyncMode.On;

        using var context = new ABContext(flexFramework, resourceManager);

#if DEBUG_SKIP_MENU
        flexFramework.LoadScene(new GameScene(context));
#else
        flexFramework.LoadScene(new IntroScene(context));
#endif

        while (!flexFramework.ShouldClose())
        {
            context.Update();
            flexFramework.Update();
        }
    }

    private static void OnLog(LogLevel level, string name, string message, Exception? exception)
    {
        if (level < LogLevel.Info)
            return;
        
        switch (level)
        {
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{name}] {message}");
                Console.ResetColor();
                break;
            case LogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{name}] {message}");
                Console.ResetColor();
                break;
            default:
                Console.WriteLine($"[{name}] {message}");
                break;
        }
    }
}
