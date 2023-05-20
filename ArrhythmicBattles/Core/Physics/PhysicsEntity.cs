using ArrhythmicBattles.Util;
using BepuPhysics;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Core.Physics;

public class PhysicsEntity : Entity, IDisposable
{
    public BodyHandle Handle { get; }
    public BodyReference Reference => physicsWorld.Simulation.Bodies.GetBodyReference(Handle);

    public Vector3 Position
    {
        get => Reference.Pose.Position.ToOpenTK();
        set => Reference.Pose.Position = value.ToSystem();
    }

    public Quaternion Rotation
    {
        get => Reference.Pose.Orientation.ToOpenTK();
        set => Reference.Pose.Orientation = value.ToSystem();
    }
    
    private readonly PhysicsWorld physicsWorld;
    
    public PhysicsEntity(PhysicsWorld physicsWorld, BodyDescription bodyDescription)
    {
        this.physicsWorld = physicsWorld;
        Handle = physicsWorld.Simulation.Bodies.Add(bodyDescription);
    }
    
    public void Dispose()
    {
        // Remove the body from the simulation
        physicsWorld.Simulation.Bodies.Remove(Handle);
    }
}