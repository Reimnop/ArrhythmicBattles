using FlexFramework.Core.UserInterface.Renderables;

namespace FlexFramework.Core.UserInterface.Elements;

public class EmptyElement : Element
{
    public override void BuildRenderables(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds constraintBounds)
    {
        Bounds boundingBox = CalculateBoundingBox(constraintBounds);
        Bounds elementBounds = CalculateElementBounds(boundingBox);
        
#if DEBUG && DEBUG_SHOW_BOUNDING_BOXES // Add bounding box drawable if in debug mode
        renderables.Add(new BoundingBoxRenderable(engine, boundingBox));
#endif
        
        Bounds contentBounds = CalculateContentBounds(elementBounds);
        float y = contentBounds.Y0;
        
        // Render children
        foreach (Element child in Children)
        {
            Bounds childConstraintBounds = new Bounds(contentBounds.X0, y, contentBounds.X1, contentBounds.Y1);
            Bounds childBounds = child.CalculateBoundingBox(childConstraintBounds);
            y += childBounds.Height;

            child.BuildRenderables(renderables, engine, childConstraintBounds);
        }
    }
}