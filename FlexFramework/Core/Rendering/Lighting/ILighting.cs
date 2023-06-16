using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Lighting;

public interface ILighting
{
    Vector3 GetAmbientLight();
    DirectionalLight GetDirectionalLight();
    IEnumerable<PointLight> GetPointLights();
}