using FlexFramework.Core;
using FlexFramework.Core.Rendering;

namespace ArrhythmicBattles.Core;

public abstract class ABScene : Scene
{
    public ABContext Context { get; }

    // Rendering stuff
    private CommandList commandList = new();

    public ABScene(ABContext context) : base(context.Engine)
    {
        Context = context;
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
}