using FlexFramework.Core.Rendering.Data;

namespace FlexFramework.Core.Rendering.Strategy;

public abstract class RenderStrategy : IDisposable
{
    protected T EnsureDrawDataType<T>(IDrawData drawData)
    {
        if (drawData is T castedDrawData)
        {
            return castedDrawData;
        }
        throw new InvalidCastException();
    }
    
    public abstract void Draw(GLStateManager glStateManager, IDrawData drawData);
    public abstract void Dispose();
}