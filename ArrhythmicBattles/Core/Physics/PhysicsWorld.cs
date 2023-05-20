using System.Numerics;
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
    public event Action? Step;
    
    private readonly BufferPool bufferPool;
    private readonly ThreadDispatcher threadDispatcher;
    private readonly Simulation simulation;
    
    private readonly Dictionary<IShape, TypedIndex> shapeIndexMap = new Dictionary<IShape, TypedIndex>();

    private float t = 0.0f;
    
    public PhysicsWorld()
    {
        bufferPool = new BufferPool();
        threadDispatcher = new ThreadDispatcher(Environment.ProcessorCount - 2);
        
        simulation = Simulation.Create(bufferPool, 
            new NarrowPhaseCallbacks(new SpringSettings(30.0f, 1.0f)), 
            new PoseIntegratorCallbacks(new Vector3(0.0f, -19.62f, 0.0f), 0.1f, 0.1f), 
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
        // This is to prevent the simulation from getting too far behind
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
        ((IDisposable) bufferPool).Dispose();
    }
}