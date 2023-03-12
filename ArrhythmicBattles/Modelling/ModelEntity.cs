using System.Diagnostics;
using ArrhythmicBattles.Modelling.Animate;
using ArrhythmicBattles.Core;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Modelling;

public class ModelEntity : Entity, IRenderable
{
    public Model? Model
    {
        get => model;
        set
        {
            model = value;
            animationHandler = null;
            animation = null;
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

    public Color4 Color { get; set; } = Color4.White;

    private Model? model;
    private AnimationHandler? animationHandler;
    private ModelAnimation? animation;

    private readonly LitMeshEntity meshEntity;

    private float time = 0.0f;

    public ModelEntity()
    {
        meshEntity = new LitMeshEntity();
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
    }

    public void Render(RenderArgs args)
    {
        if (Model == null)
        {
            return;
        }
        
        RenderModelRecursively(Model.Tree.RootNode, args);
    }
    
    // more recursion bullshit
    private void RenderModelRecursively(ImmutableNode<ModelNode> node, RenderArgs args)
    {
        Debug.Assert(model != null);

        MatrixStack matrixStack = args.MatrixStack;
        ModelNode modelNode = node.Value;

        matrixStack.Push();
        matrixStack.Transform(GetNodeTransform(modelNode));

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

    private Matrix4 GetNodeTransform(ModelNode node)
    {
        return animationHandler?.GetNodeTransform(node) ?? node.Transform;
    }
}