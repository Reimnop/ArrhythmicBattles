using ArrhythmicBattles.UserInterface;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Core;

public abstract class ABScene : Scene, IDisposable
{
    public Bounds ScreenBounds { get; protected set; }

    public ABContext Context { get; }
    
    protected MatrixStack MatrixStack { get; } = new MatrixStack();
    protected GuiCamera GuiCamera { get; }

    protected LayeredScreenHandler ScreenHandler { get; } = new LayeredScreenHandler();
    
    private List<IUpdateable> updateables = new List<IUpdateable>();
    private List<IDisposable> disposables = new List<IDisposable>();

    public ABScene(ABContext context) : base(context.Engine)
    {
        Context = context;
        
        Renderer renderer = Engine.Renderer;
        renderer.ClearColor = new Color4(33, 33, 33, 255);
        GuiCamera = new GuiCamera(Engine);
    }

    protected void RegisterObject(object obj)
    {
        if (obj is IUpdateable updateable)
        {
            updateables.Add(updateable);
        }
        
        if (obj is IDisposable disposable)
        {
            disposables.Add(disposable);
        }
    }

    public override void Update(UpdateArgs args)
    {
        ScreenHandler.Update(args);
        
        foreach (IUpdateable updateable in updateables)
        {
            updateable.Update(args);
        }
    }

    public virtual void OpenScreen(Screen screen)
    {
        ScreenHandler.OpenScreen(screen);
    }

    public virtual void CloseScreen(Screen screen)
    {
        ScreenHandler.CloseScreen(screen);
    }

    public virtual void SwitchScreen(Screen before, Screen after)
    {
        ScreenHandler.SwitchScreen(before, after);
    }

    public virtual void Dispose()
    {
        ScreenHandler.Dispose();
        
        foreach (IDisposable disposable in disposables)
        {
            disposable.Dispose();
        }
    }
}