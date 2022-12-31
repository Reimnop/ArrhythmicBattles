using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;

namespace FlexFramework.Core.EntitySystem;

public interface IRenderable
{
    void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData);
}