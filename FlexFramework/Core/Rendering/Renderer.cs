using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering;

public abstract class Renderer
{
    public abstract GpuInfo GpuInfo { get; }
    public Color4 ClearColor { get; set; } = Color4.Black;

    public abstract IRenderBuffer CreateRenderBuffer(Vector2i size);
    public abstract void Update(UpdateArgs args);
    public abstract void Render(Vector2i size, CommandList commandList, IRenderBuffer renderBuffer);
}