using ArrhythmicBattles.Modelling;
using ArrhythmicBattles.Util;
using BepuPhysics;
using BepuPhysics.Collidables;
using FlexFramework.Core.System;
using FlexFramework.Core.System.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Util;
using FlexFramework.Physics;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game;

public class PhysicsEntity : Entity, IRenderable, IDisposable
{
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    
    private readonly Model model;
    private readonly ModelEntity modelEntity;
    
    private readonly Simulation simulation;
    private readonly BodyHandle bodyHandle;
    
    public PhysicsEntity(Simulation simulation, Model model, TypedIndex capsule, BodyInertia inertia, Vector3 position, Quaternion rotation)
    {
        this.simulation = simulation;
        this.model = model;
        
        Position = position;
        Rotation = rotation;
        modelEntity = new ModelEntity();
        modelEntity.Model = model;
        
        RigidPose pose = new RigidPose(Position.ToSystem(), Rotation.ToSystem());

        BodyDescription bodyDescription = BodyDescription.CreateDynamic(
            pose, inertia,
            capsule, 
            0.02f);
        
        bodyHandle = simulation.Bodies.Add(bodyDescription);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        BodyReference bodyReference = simulation.Bodies.GetBodyReference(bodyHandle);
        RigidPose pose = bodyReference.Pose;
        
        Position = pose.Position.ToOpenTK();
        Rotation = pose.Orientation.ToOpenTK();
    }
    
    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Scale(0.5f, 0.5f, 0.5f);
        matrixStack.Rotate(Rotation);
        matrixStack.Translate(Position);
        modelEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        model.Dispose();
        simulation.Bodies.Remove(bodyHandle);
    }
}