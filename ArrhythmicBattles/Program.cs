﻿using ArrhythmicBattles.Core.Animation;
using ArrhythmicBattles.Menu;
using FlexFramework;
using FlexFramework.Core.Rendering.Renderers;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using FlexFramework.Logging;
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

        using var flexFramework = new FlexFrameworkMain(nws);
        flexFramework.Log += OnLog;
        flexFramework.VSync = VSyncMode.On;
        flexFramework.UseRenderer(new DefaultRenderer());

        using var context = new ABContext(flexFramework);

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
        string msg = $"[FlexFramework -> {sender.GetType().Name}] {eventArgs.Message}";

        switch (eventArgs.Severity)
        {
            case Severity.Debug:
                Log("DEBUG", msg);
                break;
            case Severity.Info:
                Log("INFO", msg);
                break;
            case Severity.Warning:
                Log("WARN", msg);
                break;
            case Severity.Error:
                Log("ERROR", msg);
                break;
            case Severity.Fatal:
                Log("FATAL", msg);
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