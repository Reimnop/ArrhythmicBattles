using OpenTK.Mathematics;

namespace FlexFramework.Core.Util;

public class GuiCamera : Camera
{
    private readonly FlexFrameworkMain engine;

    public override double DepthNear { get; set; } = -10.0;
    public override double DepthFar { get; set; } = 10.0;
    
    public GuiCamera(FlexFrameworkMain engine)
    {
        this.engine = engine;
    }

    public override CameraData GetCameraData(Vector2i viewportSize)
    {
        Matrix4d view = Matrix4d.Invert(Matrix4d.CreateFromQuaternion(Rotation) * Matrix4d.CreateTranslation(Position));
        Matrix4d projection = Matrix4d.CreateOrthographicOffCenter(0.0, engine.ClientSize.X, engine.ClientSize.Y, 0.0, DepthNear, DepthFar);
        
        return new CameraData(view, projection);
    }
}