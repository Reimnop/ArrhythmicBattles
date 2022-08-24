using FlexFramework.Rendering.Data;

namespace FlexFramework.Rendering;

public abstract class RenderingStrategy : IDisposable
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