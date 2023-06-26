using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;

namespace ArrhythmicBattles.Util;

public struct SimpleRayHitHandler : IRayHitHandler
{
    public CollidableReference? Hit { get; private set; } = null;

    private readonly CollidableReference collidable;
        
    public SimpleRayHitHandler(CollidableReference collidable)
    {
        this.collidable = collidable;
    }
        
    public bool AllowTest(CollidableReference collidable)
    {
        return collidable != this.collidable;
    }

    public bool AllowTest(CollidableReference collidable, int childIndex)
    {
        return collidable != this.collidable;
    }

    public void OnRayHit(in RayData ray, ref float maximumT, float t, in System.Numerics.Vector3 normal, CollidableReference collidable, int childIndex)
    {
        Hit = collidable;
    }
}