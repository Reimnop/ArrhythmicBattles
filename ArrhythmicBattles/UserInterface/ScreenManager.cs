using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UserInterface;

public delegate void OpenScreenEventHandler(Screen screen);
public delegate void CloseScreenEventHandler(Screen screen);
public delegate void SwitchScreenEventHandler(Screen before, Screen after);

public delegate InterfaceTreeBuilder InterfaceFactory(InterfaceTreeBuilder child);

public class ScreenManager : IUpdateable, IRenderable, IDisposable
{
    public event OpenScreenEventHandler? OpenScreen;
    public event CloseScreenEventHandler? CloseScreen;
    public event SwitchScreenEventHandler? SwitchScreen;
    
    public IReadOnlyList<Screen> Screens => screens;
    
    private readonly List<Screen> screens = new();
    private readonly List<Screen> currentScreens = new();

    private Box2 bounds;
    private readonly InterfaceFactory interfaceFactory;

    public ScreenManager(Box2 bounds, InterfaceFactory interfaceFactory)
    {
        this.bounds = bounds;
        this.interfaceFactory = interfaceFactory;
    }
    
    public Node<ElementContainer> BuildInterface(InterfaceTreeBuilder child)
    {
        var node = interfaceFactory(child).Build();
        LayoutEngine.Layout(node, bounds);
        
        return node;
    }

    public void Open(Screen screen)
    {
        screens.Add(screen);
        
        OpenScreen?.Invoke(screen);
    }
    
    public void Close(Screen screen)
    {
        if (screen is IDisposable disposable)
        {
            disposable.Dispose();
        }
        screens.Remove(screen);
        
        CloseScreen?.Invoke(screen);
    }
    
    public void Switch(Screen before, Screen after)
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
        
        SwitchScreen?.Invoke(before, after);
    }

    public void Resize(Box2 bounds)
    {
        this.bounds = bounds;

        foreach (var rootNode in screens.Select(screen => screen.RootNode).Where(rootNode => rootNode is not null))
        {
            LayoutEngine.Layout(rootNode!, bounds);
        }
    }
    
    public void Update(UpdateArgs args)
    {
        currentScreens.Clear();
        currentScreens.AddRange(screens);

        foreach (var screen in currentScreens)
        {
            screen.Update(args);
        }
    }

    public void Render(RenderArgs args)
    {
        foreach (Screen screen in screens)
        {
            screen.Render(args);
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