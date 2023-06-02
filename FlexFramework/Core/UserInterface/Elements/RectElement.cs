using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class RectElement : VisualElement, IRenderable
{
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

    protected override void UpdateLayout(Box2 bounds)
    {
        rectEntity.Min = bounds.Min;
        rectEntity.Max = bounds.Max;
    }

    public override void Render(RenderArgs args)
    {
        rectEntity.Render(args);
    }
}