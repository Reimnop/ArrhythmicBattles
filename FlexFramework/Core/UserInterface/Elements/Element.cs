using FlexFramework.Core.UserInterface.Renderables;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class Element
{
    public List<Element> Children { get; } = new List<Element>();

    public Length Width { get; set; } = Length.Zero;
    public Length Height { get; set; } = Length.Zero;
    public Length MarginLeft { get; set; } = Length.Zero;
    public Length MarginRight { get; set; } = Length.Zero;
    public Length MarginTop { get; set; } = Length.Zero;
    public Length MarginBottom { get; set; } = Length.Zero;
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

    public abstract void BuildRenderables(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds constraintBounds);
    
    public List<IRenderable> BuildRenderables(FlexFrameworkMain engine, Bounds constraintBounds)
    {
        List<IRenderable> renderables = new List<IRenderable>();
        BuildRenderables(renderables, engine, constraintBounds);
        return renderables;
    }
}