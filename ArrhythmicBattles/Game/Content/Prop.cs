using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content;

public abstract class Prop
{
    public abstract PropInstance CreateInstance(ResourceManager resourceManager, PhysicsWorld physicsWorld, Vector3 position, Vector3 scale, Quaternion rotation);
}
