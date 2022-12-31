using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UI;

public abstract class Screen : IDisposable
{
    public abstract Vector2 Position { get; set; }

    public abstract void Update(UpdateArgs args);
    public abstract void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData);
    public abstract void Dispose();
}