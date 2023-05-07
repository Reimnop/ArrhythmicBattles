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
        ModelNode modelNode = node.Value;
        
        matrixStack.Push();
        matrixStack.Transform(AnimationHandler.GetNodeTransform(modelNode));

        if (model.BoneIndexMap.TryGetValue(modelNode.Name, out int boneIndex))
        {
            var bone = model.Bones[boneIndex];
            var offset = bone.Offset;

            boneMatrices[boneIndex] = offset * matrixStack.GlobalTransformation * globalInverseTransform;
        }

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
        Debug.Assert(model != null);
        
        MatrixStack matrixStack = args.MatrixStack;
        ModelNode modelNode = node.Value;
        
        foreach (ModelMesh modelMesh in modelNode.Meshes)
        {
            ModelMaterial material = model.Materials[modelMesh.MaterialIndex];
            
            meshEntity.Mesh = model.SkinnedMeshes[modelMesh.MeshIndex];
            meshEntity.Color = new Color4(material.Color.R * Color.R, material.Color.G * Color.G, material.Color.B * Color.B, material.Color.A * Color.A);
            meshEntity.Texture = material.Texture;
            meshEntity.Render(args);
        }

        foreach (ImmutableNode<ModelNode> child in node.Children)
        {
            RenderModelRecursively(child, args);
        }
    }
}