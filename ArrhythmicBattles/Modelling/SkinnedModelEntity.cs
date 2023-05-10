﻿using System.Diagnostics;
using ArrhythmicBattles.Core;
using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Modelling;

// TODO: Implement support for non-zero mesh origins
public class SkinnedModelEntity : Entity, IRenderable
{
    public AnimationHandler AnimationHandler { get; }

    private readonly Model model;
    private readonly Matrix4[] boneMatrices;
    private readonly Matrix4[] inverseBindMatrices;

    private readonly MatrixStack boneMatrixStack = new();

    private float time = 0.0f;

    public SkinnedModelEntity(Model model)
    {
        this.model = model;
        boneMatrices = new Matrix4[model.Bones.Count];
        inverseBindMatrices = new Matrix4[model.Bones.Count];
        
        AnimationHandler = new AnimationHandler(model);
        
        CalculateInverseBindMatricesRecursively(model.RootNode, boneMatrixStack);
        boneMatrixStack.AssertEmpty();
    }

    private void CalculateInverseBindMatricesRecursively(ImmutableNode<ModelNode> node, MatrixStack matrixStack)
    {
        var modelNode = node.Value;
        
        matrixStack.Push();
        matrixStack.Transform(modelNode.Transform);
        // matrixStack.Transform(modelNode.Transform);
        
        if (model.BoneIndexMap.TryGetValue(modelNode.Name, out int boneIndex))
        {
            var bone = model.Bones[boneIndex];
            
            inverseBindMatrices[boneIndex] = Matrix4.Invert(bone.Offset * matrixStack.GlobalTransformation);
        }

        foreach (var child in node.Children)
        {
            CalculateInverseBindMatricesRecursively(child, matrixStack);
        }
        
        matrixStack.Pop();
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
        
        matrixStack.Push();
        matrixStack.Transform(AnimationHandler.GetNodeTransform(modelNode));

        if (model.BoneIndexMap.TryGetValue(modelNode.Name, out int boneIndex))
        {
            boneMatrices[boneIndex] = model.Bones[boneIndex].Offset * matrixStack.GlobalTransformation;
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
        var matrixStack = args.MatrixStack;
        var modelNode = node.Value;

        foreach (var modelMesh in modelNode.Meshes)
        {
            var material = model.Materials[modelMesh.MaterialIndex];

            Renderer renderer = args.Renderer;
            int layerId = args.LayerId;
            CameraData cameraData = args.CameraData;

            MaterialData materialData = new MaterialData()
            {
                UseAlbedoTexture = material.AlbedoTexture != null,
                UseMetallicTexture = material.MetallicTexture != null,
                UseRoughnessTexture = material.RoughnessTexture != null,
                Albedo = material.Albedo,
                Metallic = material.Metallic,
                Roughness = material.Roughness
            };
        
            SkinnedVertexDrawData vertexDrawData = new SkinnedVertexDrawData(
                model.SkinnedMeshes[modelMesh.MeshIndex].ReadOnly, 
                matrixStack.GlobalTransformation, cameraData,
                boneMatrices,
                material.AlbedoTexture?.ReadOnly, material.MetallicTexture?.ReadOnly, material.RoughnessTexture?.ReadOnly,
                materialData);
        
            renderer.EnqueueDrawData(layerId, vertexDrawData);
        }

        foreach (ImmutableNode<ModelNode> child in node.Children)
        {
            RenderModelRecursively(child, args);
        }
    }
}