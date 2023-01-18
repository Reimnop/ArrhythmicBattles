using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using FlexFramework.Core.Util;
using FlexFramework.Logging;

namespace FlexFramework.Physics;

public class PhysicsManager : IDisposable
{
    public Simulation Simulation => simulation;
    
    public float TimeStep { get; set; } = 1.0f / 50.0f;
    public event Action? Step;

    private readonly FlexFrameworkMain engine;
    private readonly BufferPool bufferPool;
    private readonly ThreadDispatcher threadDispatcher;
    private readonly Simulation simulation;
    
    private float t = 0.0f;
    
    public PhysicsManager(FlexFrameworkMain engine)
    {
        this.engine = engine;
        
        bufferPool = new BufferPool();
        threadDispatcher = new ThreadDispatcher(4);
        
        simulation = Simulation.Create(bufferPool, 
            new NarrowPhaseCallbacks(new SpringSettings(30.0f, 1.0f)), 
            new PoseIntegratorCallbacks(new Vector3(0.0f, -9.81f, 0.0f), 0.0f, 0.0f), 
            new SolveDescription(8, 1));

        TypedIndex shapeIndex = simulation.Shapes.Add(new Box(20.0f, 0.1f, 20.0f));

        // Add a floor
        simulation.Bodies.Add(BodyDescription.CreateKinematic(
            RigidPose.Identity, 
            shapeIndex,
            0.01f));
    }

    public void Update(UpdateArgs args)
    {
        t += args.DeltaTime;

        UpdatePhysics();
    }

    private void UpdatePhysics()
    {
        int steps = 0;
        while (t >= TimeStep)
        {
            if (steps > 2)
            {
                engine.LogMessage(this, Severity.Warning, null, "Physics is running behind!");
                t = 0.0f;
                break;
            }
            
            simulation.Timestep(TimeStep, threadDispatcher);
            Step?.Invoke();
            t -= TimeStep;
            steps++;
        }
    }

    public void Dispose()
    {
        simulation.Dispose();
        threadDispatcher.Dispose();
        ((IDisposable) bufferPool).Dispose();
    }
}