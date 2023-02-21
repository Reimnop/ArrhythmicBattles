using FlexFramework.Core.UserInterface.Renderables;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class RectElement : EmptyElement
{
    public float Radius { get; set; } = 0.0f;
    public Color4 Color { get; set; } = Color4.White;
    
    public RectElement(params Element[] children) : base(children)
    {
    }

    public override void BuildRenderables(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds constraintBounds)
    {
        CalculateBounds(renderables, engine, constraintBounds, out _, out Bounds elementBounds, out _);
        renderables.Add(new RectRenderable(engine, elementBounds, Color, Radius));
        
        base.BuildRenderables(renderables, engine, constraintBounds);
    }
}