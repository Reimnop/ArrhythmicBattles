namespace FlexFramework.Core.Rendering.Data;

public abstract class GpuObject : IDisposable
{
    public int Handle { get; }
    public string Name { get; }

    public abstract void Dispose();
}