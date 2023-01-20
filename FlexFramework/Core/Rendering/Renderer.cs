using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.PostProcessing;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering;

public abstract class Renderer : IDisposable
{
    public abstract GpuInfo GpuInfo { get; }
    public Color4 ClearColor { get; set; } = Color4.Black;

    protected FlexFrameworkMain Engine { get; private set; } = null!;

    internal void SetEngine(FlexFrameworkMain engine)
    {
        Engine = engine;
    }

    public abstract void Init();
    public abstract int GetLayerId(string name);
    public abstract void EnqueueDrawData(int layerId, IDrawData drawData);
    public abstract void UsePostProcessor(PostProcessor postProcessor);
    // TODO: Add UseBackgroundRenderer
    public abstract void Update(UpdateArgs args);
    public abstract void Render();
    public abstract void Dispose();
}