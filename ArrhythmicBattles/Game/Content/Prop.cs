using ArrhythmicBattles.Core;
using ArrhythmicBattles.Core.Physics;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content;

public abstract class Prop
{
    protected ContentLoader ContentLoader { get; }
    protected PhysicsWorld PhysicsWorld { get; }
    protected Vector3 InitialPosition { get; }
    protected Vector3 InitialScale { get; }
    protected Quaternion InitialRotation { get; }

    protected Prop(ContentLoader contentLoader, PhysicsWorld physicsWorld, Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation)
    {
        ContentLoader = contentLoader;
        PhysicsWorld = physicsWorld;
        InitialPosition = initialPosition;
        InitialScale = initialScale;
        InitialRotation = initialRotation;
    }
}