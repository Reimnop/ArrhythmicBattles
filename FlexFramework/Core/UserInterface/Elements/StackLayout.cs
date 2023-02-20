using FlexFramework.Core.UserInterface.Renderables;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class StackLayout : Element
{
    public Length Spacing { get; set; } = Length.Zero;

    public StackLayout(params Element[] children)
    {
        Children.AddRange(children);
    }

    public override void BuildRenderables(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds constraintBounds)
    {
        Bounds boundingBox = CalculateBoundingBox(constraintBounds);
        Bounds elementBounds = CalculateElementBounds(boundingBox);
        Bounds contentBounds = CalculateContentBounds(elementBounds);
        
#if DEBUG && DEBUG_SHOW_BOUNDING_BOXES // Add bounding box drawable if in debug mode
        renderables.Add(new BoundingBoxRenderable(engine, boundingBox, Color4.White));
        renderables.Add(new BoundingBoxRenderable(engine, elementBounds, Color4.Red));
        renderables.Add(new BoundingBoxRenderable(engine, contentBounds, Color4.Lime));
#endif

        float spacing = Spacing.Calculate(contentBounds.Height);

        // Create child drawables
        float y = contentBounds.Y0;
        foreach (Element child in Children)
        {
            // The child bounds are constrained to the parent bounds
            Bounds childConstraintBounds = new Bounds(contentBounds.X0, y, contentBounds.X1, contentBounds.Y1);

            // Calculate the child bounds
            Bounds childBounds = child.CalculateBoundingBox(childConstraintBounds);
            y += childBounds.Height + spacing; // Add the spacing to the y position

            // Add the child drawables
            child.BuildRenderables(renderables, engine, childConstraintBounds);
        }
    }
}