using ArrhythmicBattles;
using FlexFramework;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using ArrhythmicBattles.MainMenu;
using ArrhythmicBattles.Util;
using OpenTK.Windowing.Common.Input;
using StbImageSharp;

Image GetImageFromFile(string path)
{
    using FileStream stream = File.OpenRead(path);
    ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
    return new Image(result.Width, result.Height, result.Data);
}

Image imageS = GetImageFromFile("Assets/Icons/icon_s.png");
Image imageM = GetImageFromFile("Assets/Icons/icon_m.png");
Image imageL = GetImageFromFile("Assets/Icons/icon_l.png");
Image imageXl = GetImageFromFile("Assets/Icons/icon_xl.png");

WindowIcon icon = new WindowIcon(imageS, imageM, imageL, imageXl);

NativeWindowSettings nws = new NativeWindowSettings()
{
    Title = "Arrhythmic Battles",
    Size = new Vector2i(1366, 768),
    APIVersion = new Version(3, 2),
    Profile = ContextProfile.Core,
    NumberOfSamples = 4,
    Icon = icon
};

using FlexFrameworkMain flexFramework = new FlexFrameworkMain(nws);

using ABContext context = new ABContext(flexFramework);

flexFramework.UseRenderer<DefaultRenderer>();
flexFramework.LoadFonts(2048,
    new FontFileInfo("inconsolata-regular", 24, "Assets/Fonts/Inconsolata-Regular.ttf"), 
    new FontFileInfo("inconsolata-small", 18, "Assets/Fonts/Inconsolata-Regular.ttf"));
flexFramework.LoadScene<MainMenuScene>(context);

while (!flexFramework.ShouldClose())
{
    flexFramework.Update();
}