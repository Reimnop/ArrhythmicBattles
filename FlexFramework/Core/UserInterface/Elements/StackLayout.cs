using FlexFramework.Core.UserInterface.Drawables;

namespace FlexFramework.Core.UserInterface.Elements;

public class StackLayout : Element
{
    public Length Spacing { get; set; } = Length.Zero;

    public StackLayout(params Element[] children)
    {
        Children.AddRange(children);
    }

    public override void BuildRenderables(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds elementBounds)
    {
#if DEBUG && DEBUG_SHOW_BOUNDING_BOXES // Add bounding box drawable if in debug mode
        renderables.Add(new BoundingBoxRenderable(engine, elementBounds));
#endif
        
        // Get the bounds of this element content area
        Bounds bounds = CalculatePaddingBounds(elementBounds);
        float spacing = Spacing.Calculate(bounds.Height);

        // Create child drawables
        float y = bounds.Y0;
        foreach (Element child in Children)
        {
            // The child bounds are constrained to the parent bounds
            Bounds childParentBounds = new Bounds(bounds.X0, y, bounds.X1, bounds.Y1);

            // Calculate the actual child bounds
            Bounds childBounds = child.CalculateBounds(childParentBounds);

            // Expand child bounds to fill the parent bounds
            childBounds.Width = childParentBounds.Width;

            // Add the child drawables
            child.BuildRenderables(renderables, engine, childBounds);

            // Add the spacing
            y += childBounds.Height + spacing;
        }
    }
}