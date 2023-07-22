using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UserInterface;

public delegate void OpenScreenEventHandler(IScreen screen);
public delegate void CloseScreenEventHandler(IScreen screen);
public delegate void SwitchScreenEventHandler(IScreen before, IScreen after);

public delegate InterfaceTreeBuilder InterfaceFactory(InterfaceTreeBuilder child);

public class ScreenManager : IUpdateable, IRenderable, IDisposable
{
    public Box2 ComputedBounds => new(bounds.Min / dpiScale, bounds.Max / dpiScale);
    
    public event OpenScreenEventHandler? OpenScreen;
    public event CloseScreenEventHandler? CloseScreen;
    public event SwitchScreenEventHandler? SwitchScreen;
    
    public IReadOnlyList<IScreen> Screens => screens;
    
    private readonly List<IScreen> screens = new();
    private readonly List<IScreen> currentScreens = new();

    private Box2 bounds;
    private float dpiScale;
    private readonly InterfaceFactory interfaceFactory;

    public ScreenManager(Box2 bounds, float dpiScale, InterfaceFactory interfaceFactory)
    {
        this.bounds = bounds;
        this.dpiScale = dpiScale;
        this.interfaceFactory = interfaceFactory;
    }
    
    public Node<ElementContainer> BuildInterface(InterfaceTreeBuilder child)
    {
        var node = interfaceFactory(child).Build();
        LayoutEngine.Layout(node, ComputedBounds, dpiScale);
        
        return node;
    }
    
    public void Resize(Box2 bounds, float dpiScale)
    {
        this.bounds = bounds;
        this.dpiScale = dpiScale;

        foreach (var rootNode in screens.Select(screen => screen.RootNode).Where(rootNode => rootNode is not null))
        {
            LayoutEngine.Layout(rootNode!, ComputedBounds, dpiScale);
        }
    }

    public void Open(IScreen screen)
    {
        screens.Add(screen);
        
        OpenScreen?.Invoke(screen);
    }
    
    public void Close(IScreen screen)
    {
        if (screen is IDisposable disposable)
        {
            disposable.Dispose();
        }
        screens.Remove(screen);
        
        CloseScreen?.Invoke(screen);
    }
    
    public void Switch(IScreen before, IScreen after)
    {
        var index = screens.IndexOf(before);
        if (index == -1)
        {
            throw new ArgumentException("Screen not found!", nameof(before));
        }
        
        if (before is IDisposable disposable)
        {
            disposable.Dispose();
        }
        screens[index] = after;
        
        SwitchScreen?.Invoke(before, after);
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
        foreach (var screen in screens)
        {
            screen.Render(args);
        }
    }

    public void Dispose()
    {
        foreach (var disposable in screens.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}