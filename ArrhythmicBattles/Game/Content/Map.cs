using FlexFramework.Core;

namespace ArrhythmicBattles.Game.Content;

// Contains metadata, props and soundtrack
public class Map : IUpdateable, IRenderable, IDisposable
{
    
    
    private readonly List<Prop> props = new();
    
    public Map(IEnumerable<Prop> props)
    {
        this.props.AddRange(props);
    }

    public void Update(UpdateArgs args)
    {
        foreach (var updateable in props.OfType<IUpdateable>())
        {
            updateable.Update(args);
        }
    }

    public void Render(RenderArgs args)
    {
        foreach (var renderable in props.OfType<IRenderable>())
        {
            renderable.Render(args);
        }
    }

    public void Dispose()
    {
        foreach (var disposable in props.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}