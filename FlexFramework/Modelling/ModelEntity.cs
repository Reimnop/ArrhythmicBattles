﻿using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;

namespace FlexFramework.Modelling;

public class ModelEntity : Entity, IRenderable
{
    public AnimationHandler AnimationHandler { get; }

    private readonly Model model;

    private float time = 0.0f;

    public ModelEntity(Model model)
    {
        this.model = model;
        
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
    private void RenderModelRecursively(Node<ModelNode> node, RenderArgs args)
    {
        var commandList = args.CommandList;
        var layerType = args.LayerType;
        var matrixStack = args.MatrixStack;
        var cameraData = args.CameraData;
        var modelNode = node.Value;

        matrixStack.Push();
        matrixStack.Transform(AnimationHandler.GetNodeTransform(modelNode));

        foreach (var modelMesh in modelNode.Meshes)
        {
            var material = model.Materials[modelMesh.MaterialIndex];

            var materialData = new MaterialData
            {
                UseAlbedoTexture = material.AlbedoTexture != null,
                UseMetallicTexture = material.MetallicTexture != null,
                UseRoughnessTexture = material.RoughnessTexture != null,
                Albedo = material.Albedo,
                Metallic = material.Metallic,
                Roughness = material.Roughness
            };
        
            var vertexDrawData = new LitVertexDrawData(
                model.Meshes[modelMesh.MeshIndex].ReadOnly, 
                matrixStack.GlobalTransformation, 
                cameraData, 
                material.AlbedoTexture?.ReadOnly, 
                material.MetallicTexture?.ReadOnly, 
                material.RoughnessTexture?.ReadOnly,
                materialData);
            
            commandList.AddDrawData(layerType, vertexDrawData);
        }

        foreach (Node<ModelNode> child in node.Children)
        {
            RenderModelRecursively(child, args);
        }
        
        matrixStack.Pop();
    }
}