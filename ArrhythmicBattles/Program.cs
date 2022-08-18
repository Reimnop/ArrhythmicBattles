using FlexFramework;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using ArrhythmicBattles.MainMenu;

NativeWindowSettings nws = new NativeWindowSettings()
{
    Title = "FlexFramework",
    Size = new Vector2i(1366, 768),
    APIVersion = new Version(3, 2),
    Profile = ContextProfile.Core,
    NumberOfSamples = 4
};

using (FlexFrameworkMain flexFramework = new FlexFrameworkMain(nws))
{
    flexFramework.UseRenderer<DefaultRenderer>();
    flexFramework.LoadFonts(2048,
        new FontFileInfo("inconsolata-regular", 24, "Assets/Fonts/Inconsolata-Regular.ttf"),
        new FontFileInfo("inconsolata-small", 18, "Assets/Fonts/Inconsolata-Regular.ttf"));
    flexFramework.LoadScene<MainMenuScene>();

    while (!flexFramework.ShouldClose())
    {
        flexFramework.Update();
    }
}