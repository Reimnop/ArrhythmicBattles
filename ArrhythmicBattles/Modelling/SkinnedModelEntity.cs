using System.Diagnostics;
using ArrhythmicBattles.Core;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Modelling;

public class SkinnedModelEntity : Entity, IRenderable
{
    public AnimationHandler AnimationHandler { get; }
    public Color4 Color { get; set; } = Color4.White;

    private readonly Model model;
    private readonly Matrix4 globalInverseTransform;
    private readonly Matrix4[] boneMatrices;
    private readonly SkinnedMeshEntity meshEntity;

    private readonly MatrixStack boneMatrixStack = new();

    private float time = 0.0f;

    public SkinnedModelEntity(Model model)
    {
        this.model = model;
        globalInverseTransform = Matrix4.Invert(model.RootNode.Value.Transform);
        boneMatrices = new Matrix4[model.Bones.Count];
        
        meshEntity = new SkinnedMeshEntity(boneMatrices);
        AnimationHandler = new AnimationHandler(model);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        time += args.DeltaTime;
        AnimationHandler.Update(time);
        
        CalculateBoneMatricesRecursively(model.RootNode, boneMatrixStack);
    }
    
    private void CalculateBoneMatricesRecursively(ImmutableNode<ModelNode> node, MatrixStack matrixStack)
    {
        var modelNode = node.Value;
        
        var animationTransform = AnimationHandler.GetNodeTransform(modelNode);
        if (model.BoneIndexMap.TryGetValue(modelNode.Name, out int boneIndex))
        {
            var bone = model.Bones[boneIndex];
            var offset = bone.Offset;

            boneMatrices[boneIndex] = offset * animationTransform * matrixStack.GlobalTransformation * globalInverseTransform;
        }
        
        matrixStack.Push();
        matrixStack.Transform(animationTransform);

        foreach (var child in node.Children)
        {
            CalculateBoneMatricesRecursively(child, matrixStack);
        }
        
        matrixStack.Pop();
    }

    public void Render(RenderArgs args)
    {
        RenderModelRecursively(model.RootNode, args);
    }
    
    // more recursion bullshit
    private void RenderModelRecursively(ImmutableNode<ModelNode> node, RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        var modelNode = node.Value;
        
        foreach (var modelMesh in modelNode.Meshes)
        {
            var material = model.Materials[modelMesh.MaterialIndex];
            
            meshEntity.Mesh = model.SkinnedMeshes[modelMesh.MeshIndex];
            meshEntity.Albedo = material.Albedo;
            meshEntity.Metallic = material.Metallic;
            meshEntity.Roughness = material.Roughness;
            meshEntity.AlbedoTexture = material.AlbedoTexture;
            meshEntity.MetallicTexture = material.MetallicTexture;
            meshEntity.RoughnessTexture = material.RoughnessTexture;
            meshEntity.Render(args);
        }

        foreach (ImmutableNode<ModelNode> child in node.Children)
        {
            RenderModelRecursively(child, args);
        }
    }
}