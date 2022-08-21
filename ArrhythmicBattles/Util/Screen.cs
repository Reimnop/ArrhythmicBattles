using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Util;

public abstract class Screen : IDisposable
{
    public abstract Vector2i Position { get; set; }

    public abstract void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData);
    public abstract void Update(UpdateArgs args);
    public abstract void Dispose();
}