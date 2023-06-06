using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Core;

public abstract class ABScene : Scene, IDisposable
{
    public ABContext Context { get; }
    protected EntityManager EntityManager { get; } = new();
    protected MatrixStack MatrixStack { get; } = new();
    protected GuiCamera GuiCamera { get; }

    // Rendering stuff
    private CommandList commandList = new();

    public ABScene(ABContext context) : base(context.Engine)
    {
        Context = context;

        var renderer = Engine.Renderer;
        renderer.ClearColor = Color4.Black;
        GuiCamera = new GuiCamera(Engine);
    }

    public override void Update(UpdateArgs args)
    {
        EntityManager.Update(args);
    }

    public override void Render(Renderer renderer)
    {
        // Clear command list
        commandList.Clear();
        
        // Queue render commands
        RenderScene(commandList);
        
        // Render and present
        renderer.Render(Engine.ClientSize, commandList, Context.RenderBuffer);
        Engine.Present(Context.RenderBuffer);
    }
    
    protected abstract void RenderScene(CommandList commandList);

    public virtual void Dispose()
    {
        EntityManager.Dispose();
    }
}