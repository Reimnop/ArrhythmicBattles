using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content.Props;

public class GridProp : Prop
{
    public override PropInstance CreateInstance(ResourceManager resourceManager, PhysicsWorld physicsWorld, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        return new GridPropInstance(resourceManager, physicsWorld, position, scale, rotation);
    }
}
