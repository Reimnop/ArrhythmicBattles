using FlexFramework.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Rendering.PostProcessing;

public abstract class PostProcessor : IDisposable
{
    public Vector2i CurrentSize { get; private set; }

    public virtual void Resize(Vector2i size)
    {
        CurrentSize = size;
    }
    public virtual void Init(Vector2i size)
    {
        CurrentSize = size;
    }
    public abstract void Process(GLStateManager stateManager, Texture2D texture);
    public abstract void Dispose();
}