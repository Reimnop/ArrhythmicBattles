using FlexFramework.Core.Rendering;

namespace FlexFramework.Core;

public struct RenderArgs
{
    public Renderer Renderer { get; }
    public int LayerId { get; }
    public MatrixStack MatrixStack { get; }
    public CameraData CameraData { get; }
    
    public RenderArgs(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        Renderer = renderer;
        LayerId = layerId;
        MatrixStack = matrixStack;
        CameraData = cameraData;
    }
}