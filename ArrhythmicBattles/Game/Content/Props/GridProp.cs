using FlexFramework.Core;
using FlexFramework.Modelling;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content.Props;

public class GridProp : Prop, IUpdateable, IRenderable
{
    private readonly ModelEntity entity;

    public GridProp(ContentLoader contentLoader, Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation) 
        : base(contentLoader, initialPosition, initialScale, initialRotation)
    {
        // We don't need to dispose of the model because it's managed by the ContentLoader
        var model = contentLoader.Load<Model>("Grid.dae");
        entity = new ModelEntity(model);
    }
    
    public void Update(UpdateArgs args)
    {
        entity.Update(args);
    }

    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Scale(InitialScale);
        matrixStack.Translate(InitialPosition);
        matrixStack.Rotate(InitialRotation);
        entity.Render(args);
        matrixStack.Pop();
    }
}