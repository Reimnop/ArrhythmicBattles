using FlexFramework;
using FlexFramework.Core.Rendering.Lighting;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game;

public class GameLighting : ILighting
{
    public Vector3 GetAmbientLight()
    {
        return Vector3.One * 0.25f;
    }

    public DirectionalLight GetDirectionalLight()
    {
        return new DirectionalLight(new Vector3(0.5f, -1, 0.5f).Normalized(), Vector3.One, 0.7f);
    }

    public int GetPointLightsCount() => 0;
    public IEnumerable<PointLight> GetPointLights()
    {
        yield break;
    }
}