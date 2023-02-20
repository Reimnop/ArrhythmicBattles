using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Util;

namespace ArrhythmicBattles.UI;

public class LayeredScreenHandler : Entity, IRenderable, IDisposable
{
    public IEnumerable<Screen> Screens => screens;
    
    private readonly List<Screen> screens = new List<Screen>();
    private readonly List<Screen> currentScreens = new List<Screen>();

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        currentScreens.Clear();
        currentScreens.AddRange(screens);

        foreach (Screen screen in currentScreens)
        {
            screen.Update(args);
        }
    }

    public void OpenScreen(Screen screen)
    {
        screens.Add(screen);
    }
    
    public void CloseScreen(Screen screen)
    {
        if (screen is IDisposable disposable)
        {
            disposable.Dispose();
        }
        screens.Remove(screen);
    }
    
    public void SwitchScreen(Screen before, Screen after)
    {
        int index = screens.IndexOf(before);
        if (index == -1)
        {
            throw new ArgumentException("Screen not found", nameof(before));
        }
        
        if (before is IDisposable disposable)
        {
            disposable.Dispose();
        }
        screens[index] = after;
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        foreach (Screen screen in screens)
        {
            screen.Render(renderer, layerId, matrixStack, cameraData);
        }
    }

    public void Dispose()
    {
        foreach (IDisposable disposable in screens.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}