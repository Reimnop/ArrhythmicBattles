using OpenTK.Mathematics;

namespace FlexFramework.Core.Util;

public struct CameraData
{
    public Matrix4 View { get; }
    public Matrix4 Projection { get; }

    public CameraData(Matrix4 view, Matrix4 projection)
    {
        View = view;
        Projection = projection;
    }
}