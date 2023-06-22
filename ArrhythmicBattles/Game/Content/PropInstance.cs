using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content;

public abstract class PropInstance
{
    public ResourceManager ResourceManager { get; }
    public PhysicsWorld PhysicsWorld { get; }
    public Vector3 InitialPosition { get; }
    public Vector3 InitialScale { get; }
    public Quaternion InitialRotation { get; }

    protected PropInstance(ResourceManager resourceManager, PhysicsWorld physicsWorld, Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation)
    {
        ResourceManager = resourceManager;
        PhysicsWorld = physicsWorld;
        InitialPosition = initialPosition;
        InitialScale = initialScale;
        InitialRotation = initialRotation;
    }
}