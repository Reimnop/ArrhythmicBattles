using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content;

public abstract class Prop
{
    protected ResourceManager ResourceManager { get; }
    protected PhysicsWorld PhysicsWorld { get; }
    protected Vector3 InitialPosition { get; }
    protected Vector3 InitialScale { get; }
    protected Quaternion InitialRotation { get; }

    protected Prop(ResourceManager resourceManager, PhysicsWorld physicsWorld, Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation)
    {
        ResourceManager = resourceManager;
        PhysicsWorld = physicsWorld;
        InitialPosition = initialPosition;
        InitialScale = initialScale;
        InitialRotation = initialRotation;
    }
}