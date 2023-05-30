using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class RectElement : VisualElement, IRenderable
{
    public override Vector2 Size => Vector2.Zero;
    
    public float Radius
    {
        get => rectEntity.Radius;
        set => rectEntity.Radius = value;
    }

    public Color4 Color
    {
        get => rectEntity.Color;
        set => rectEntity.Color = value;
    }

    private readonly RectEntity rectEntity = new();

    public override void SetBox(ElementBox box)
    {
        base.SetBox(box);
        
        rectEntity.Min = box.BorderBox.Min;
        rectEntity.Max = box.BorderBox.Max;
    }

    public override void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        RenderTransform.ApplyToMatrixStack(matrixStack);
        rectEntity.Render(args);
        matrixStack.Pop();
    }
}