﻿using System.Numerics;
using ArrhythmicBattles.Util;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using FlexFramework.Core;

namespace ArrhythmicBattles.Core.Physics;

public class PhysicsWorld : IDisposable, IUpdateable
{
    public Simulation Simulation => simulation;
    public float TimeStep { get; set; } = 1.0f / 50.0f;
    public float Gravity { get; }
    public float Damping { get; }

    public event Action? Step;
    
    private readonly BufferPool bufferPool;
    private readonly ThreadDispatcher threadDispatcher;
    private readonly Simulation simulation;
    
    private readonly Dictionary<IShape, TypedIndex> shapeIndexMap = new Dictionary<IShape, TypedIndex>();

    private float t = 0.0f;

    public PhysicsWorld(float gravity, float damping)
    {
        Gravity = gravity;
        Damping = damping;
        
        bufferPool = new BufferPool();
        threadDispatcher = new ThreadDispatcher(Environment.ProcessorCount - 2);
        
        simulation = Simulation.Create(bufferPool, 
            new NarrowPhaseCallbacks(new SpringSettings(30.0f, 1.0f)), 
            new PoseIntegratorCallbacks(Vector3.UnitY * gravity, damping, damping), 
            new SolveDescription(8, 1));
    }

    public void Update(UpdateArgs args)
    {
        t += args.DeltaTime;

        UpdatePhysics();
    }

    private void UpdatePhysics()
    {
        const int maxSteps = 4;
        
        int i = 0;
        while (t >= TimeStep && i < maxSteps)
        {
            simulation.Timestep(TimeStep, threadDispatcher);
            Step?.Invoke();
            t -= TimeStep;
            i++;
        }

        
        // Reset time if we've reached the maximum number of steps
        // This is to prevent the simulation from lagging too far behind
        // and causing the simulation to explode
        if (i == maxSteps)
        {
            t = 0.0f;
        }
    }

    public void Dispose()
    {
        simulation.Dispose();
        threadDispatcher.Dispose();
        
       if (bufferPool is IDisposable disposable)
       {
           disposable.Dispose();
       } 
    }
}