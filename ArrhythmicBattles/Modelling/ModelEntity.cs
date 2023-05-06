using System.Diagnostics;
using ArrhythmicBattles.Core;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Modelling;

public class ModelEntity : Entity, IRenderable
{
    public AnimationHandler AnimationHandler { get; }
    public Color4 Color { get; set; } = Color4.White;

    private readonly Model model;
    private readonly LitMeshEntity meshEntity;

    private float time = 0.0f;

    public ModelEntity(Model model)
    {
        this.model = model;
        
        meshEntity = new LitMeshEntity();
        AnimationHandler = new AnimationHandler(model);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        time += args.DeltaTime;
        AnimationHandler.Update(time);
    }

    public void Render(RenderArgs args)
    {
        RenderModelRecursively(model.RootNode, args);
    }
    
    // more recursion bullshit
    private void RenderModelRecursively(ImmutableNode<ModelNode> node, RenderArgs args)
    {
        Debug.Assert(model != null);
        
        MatrixStack matrixStack = args.MatrixStack;
        ModelNode modelNode = node.Value;

        matrixStack.Push();
        matrixStack.Transform(AnimationHandler.GetNodeTransform(modelNode));

        foreach (ModelMesh modelMesh in modelNode.Meshes)
        {
            ModelMaterial material = model.Materials[modelMesh.MaterialIndex];
            
            meshEntity.Mesh = model.Meshes[modelMesh.MeshIndex];
            meshEntity.Color = new Color4(
                material.EmissiveStrength * material.Color.R * Color.R, 
                material.EmissiveStrength * material.Color.G * Color.G, 
                material.EmissiveStrength * material.Color.B * Color.B, 
                material.EmissiveStrength * material.Color.A * Color.A);;
            meshEntity.Texture = material.Texture;
            meshEntity.Render(args);
        }

        foreach (ImmutableNode<ModelNode> child in node.Children)
        {
            RenderModelRecursively(child, args);
        }
        
        matrixStack.Pop();
    }
}