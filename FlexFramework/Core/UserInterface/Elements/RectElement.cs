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

    public override void LayoutCallback(ElementBoxes boxes)
    {
        rectEntity.Min = boxes.BorderBox.Min;
        rectEntity.Max = boxes.BorderBox.Max;
    }

    public override void Render(RenderArgs args)
    {
        rectEntity.Render(args);
    }
}