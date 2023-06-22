using ArrhythmicBattles.Core;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using ArrhythmicBattles.Util;
using BepuPhysics;
using BepuPhysics.Collidables;
using FlexFramework.Core;
using FlexFramework.Modelling;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content.Props;

public class GridPropInstance : PropInstance, IUpdateable, IRenderable, IDisposable
{
    private readonly Simulation simulation;
    
    private readonly EntityManager entityManager = new();
    private readonly ModelEntity modelEntity;

    private readonly TypedIndex shapeIndex;

    public GridPropInstance(ResourceManager resourceManager, PhysicsWorld physicsWorld, Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation) 
        : base(resourceManager, physicsWorld, initialPosition, initialScale, initialRotation)
    {
        simulation = physicsWorld.Simulation;
        
        // We don't need to dispose of the model because it's managed by the ResourceManager
        var model = resourceManager.Get<Model>("Models/Grid.dae");
        modelEntity = entityManager.Create(() => new ModelEntity(model));
        
        // Create the physics body
        var shape = new Box(20.0f * initialScale.X, 0.1f * initialScale.Y, 20.0f * initialScale.Z);
        shapeIndex = simulation.Shapes.Add(shape);
        var collidableDescription = new CollidableDescription(shapeIndex, 0.1f);
        var bodyDescription = BodyDescription.CreateKinematic(
            new RigidPose(initialPosition.ToSystem(), initialRotation.ToSystem()),
            collidableDescription,
            new BodyActivityDescription(0.01f));

        entityManager.Create(() => new PhysicsEntity(physicsWorld, bodyDescription));
    }
    
    public void Update(UpdateArgs args)
    {
        entityManager.Update(args);
    }

    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Scale(InitialScale);
        matrixStack.Translate(InitialPosition);
        matrixStack.Rotate(InitialRotation);
        entityManager.Invoke(modelEntity, entity => entity.Render(args));
        matrixStack.Pop();
    }

    public void Dispose()
    {
        entityManager.Dispose();
    }
}