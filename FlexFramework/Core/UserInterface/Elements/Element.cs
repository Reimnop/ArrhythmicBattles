using System.Diagnostics;
using FlexFramework.Core.UserInterface.Drawables;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class Element
{
    public List<Element> Children { get; } = new List<Element>();

    public Length Width { get; set; } = Length.Zero;
    public Length Height { get; set; } = Length.Zero;

    public Length Margin
    {
        set => MarginLeft = MarginRight = MarginTop = MarginBottom = value;
    }
    public Length MarginLeft { get; set; } = Length.Zero;
    public Length MarginRight { get; set; } = Length.Zero;
    public Length MarginTop { get; set; } = Length.Zero;
    public Length MarginBottom { get; set; } = Length.Zero;
    
    public Length Padding
    {
        set => PaddingLeft = PaddingRight = PaddingTop = PaddingBottom = value;
    }
    public Length PaddingLeft { get; set; } = Length.Zero;
    public Length PaddingRight { get; set; } = Length.Zero;
    public Length PaddingTop { get; set; } = Length.Zero;
    public Length PaddingBottom { get; set; } = Length.Zero;

    // Bounding box is the area that the element occupies
    public Bounds CalculateBoundingBox(Bounds constraintBounds)
    {
        return new Bounds(
            constraintBounds.X0, 
            constraintBounds.Y0, 
            constraintBounds.X0 + Width.Calculate(constraintBounds.Width), 
            constraintBounds.Y0 + Height.Calculate(constraintBounds.Height));
    }

    // Element bounds is the area that the element can draw to
    public Bounds CalculateElementBounds(Bounds boundingBox)
    {
        return new Bounds(
            boundingBox.X0 + MarginLeft.Calculate(boundingBox.Width),
            boundingBox.Y0 + MarginTop.Calculate(boundingBox.Height),
            boundingBox.X1 - MarginRight.Calculate(boundingBox.Width),
            boundingBox.Y1 - MarginBottom.Calculate(boundingBox.Height));
    }
    
    // Content bounds is the area where the element's children can draw to
    public Bounds CalculateContentBounds(Bounds elementBounds)
    {
        return new Bounds(
            elementBounds.X0 + PaddingLeft.Calculate(elementBounds.Width),
            elementBounds.Y0 + PaddingTop.Calculate(elementBounds.Height),
            elementBounds.X1 - PaddingRight.Calculate(elementBounds.Width),
            elementBounds.Y1 - PaddingBottom.Calculate(elementBounds.Height));
    }
    
    protected void CalculateBounds(Bounds constraintBounds, out Bounds boundingBox, out Bounds elementBounds, out Bounds contentBounds)
    {
        boundingBox = CalculateBoundingBox(constraintBounds);
        elementBounds = CalculateElementBounds(boundingBox);
        contentBounds = CalculateContentBounds(elementBounds);
    }
    
    [Conditional("DEBUG_SHOW_BOUNDING_BOXES")]
    protected void DrawDebugBounds(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds constraintBounds)
    {
        CalculateBounds(constraintBounds, out Bounds boundingBox, out Bounds elementBounds, out Bounds contentBounds);
        
        renderables.Add(new BoundingBoxDrawable(engine, boundingBox, Color4.White));
        renderables.Add(new BoundingBoxDrawable(engine, elementBounds, Color4.Red));
        renderables.Add(new BoundingBoxDrawable(engine, contentBounds, Color4.Lime));
    }

    public abstract void BuildDrawables(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds constraintBounds);
    
    public List<IRenderable> BuildDrawables(FlexFrameworkMain engine, Bounds constraintBounds)
    {
        List<IRenderable> renderables = new List<IRenderable>();
        BuildDrawables(renderables, engine, constraintBounds);
        return renderables;
    }
}