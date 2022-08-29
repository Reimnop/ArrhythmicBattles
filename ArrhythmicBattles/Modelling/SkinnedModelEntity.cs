using System.Diagnostics;
using ArrhythmicBattles.Modelling.Animate;
using ArrhythmicBattles.Util;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Modelling;

public class SkinnedModelEntity : Entity, IRenderable
{
    public Model? Model
    {
        get => model;
        set
        {
            model = value;
            animationHandler = null;
            animation = null;
            boneMatrices = null;

            if (model != null)
            {
                boneMatrices = new Matrix4[model.Bones.Count];
                meshEntity.Bones = boneMatrices;
            }
        }
    }

    public ModelAnimation? Animation
    {
        get => animation;
        set
        {
            animation = value;
            
            if (value == null)
            {
                animationHandler = null;
            }
            else
            {
                animationHandler = new AnimationHandler(value);
            }
        }
    }

    private Model? model;
    private AnimationHandler? animationHandler;
    private ModelAnimation? animation;
    private Matrix4[]? boneMatrices;

    private readonly SkinnedMeshEntity meshEntity;

    private readonly MatrixStack boneMatrixStack = new MatrixStack();

    private float time = 0.0f;

    public SkinnedModelEntity()
    {
        meshEntity = new SkinnedMeshEntity();
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        time += args.DeltaTime;

        if (model == null)
        {
            return;
        }

        if (animationHandler != null)
        {
            Debug.Assert(animation != null);
            animationHandler.Update(time % (animation.DurationInTicks * animation.TicksPerSecond));
        }
        
        CalculateBoneMatricesRecursively(model.Tree.RootNode, boneMatrixStack);
    }
    
    private void CalculateBoneMatricesRecursively(ImmutableNode<ModelNode> node, MatrixStack matrixStack)
    {
        Debug.Assert(model != null);
        Debug.Assert(boneMatrices != null);

        ModelNode modelNode = node.Value;
        
        matrixStack.Push();
        matrixStack.Transform(GetNodeTransform(modelNode));

        if (model.BoneIndexMap.TryGetValue(modelNode.Name, out int boneIndex))
        {
            boneMatrices[boneIndex] = model.Bones[boneIndex].Offset * matrixStack.GlobalTransformation;
        }

        foreach (ImmutableNode<ModelNode> child in node.Children)
        {
            CalculateBoneMatricesRecursively(child, matrixStack);
        }
        
        matrixStack.Pop();
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        if (Model == null)
        {
            return;
        }
        
        RenderModelRecursively(Model.Tree.RootNode, renderer, layerId, matrixStack, cameraData);
    }
    
    // more recursion bullshit
    private void RenderModelRecursively(ImmutableNode<ModelNode> node, Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        Debug.Assert(model != null);

        ModelNode modelNode = node.Value;

        foreach (ModelMesh modelMesh in modelNode.Meshes)
        {
            ModelMaterial material = model.Materials[modelMesh.MaterialIndex];
            
            meshEntity.Mesh = model.SkinnedMeshes[modelMesh.MeshIndex];
            meshEntity.Color = material.Color;
            meshEntity.Texture = material.Texture;
            meshEntity.Render(renderer, layerId, matrixStack, cameraData);
        }

        foreach (ImmutableNode<ModelNode> child in node.Children)
        {
            RenderModelRecursively(child, renderer, layerId, matrixStack, cameraData);
        }
    }

    private Matrix4 GetNodeTransform(ModelNode node)
    {
        return animationHandler?.GetNodeTransform(node) ?? node.Transform;
    }
    
    public override void Dispose()
    {
        meshEntity.Dispose();
    }
}