namespace FlexFramework.Core.UserInterface.Elements;

public class StackLayout : Element
{
    public Length Spacing { get; set; } = Length.Zero;

    public override void BuildDrawables(List<Drawable> drawables, Bounds parentBounds)
    {
        // Get the bounds of this element
        Bounds bounds = CalculateBounds(parentBounds);
        bounds.X0 += PaddingLeft.Calculate(bounds.Width);
        bounds.X1 -= PaddingRight.Calculate(bounds.Width);
        bounds.Y0 += PaddingTop.Calculate(bounds.Height);
        bounds.Y1 -= PaddingBottom.Calculate(bounds.Height);
        float spacing = Spacing.Calculate(bounds.Height);

        // Create child drawables
        float y = bounds.Y0;
        foreach (Element child in Children)
        {
            // The child bounds are constrained to the parent bounds
            Bounds childParentBounds = new Bounds(
                bounds.X0,
                y,
                bounds.X1,
                parentBounds.Y1
            );

            // Calculate the actual child bounds
            Bounds childBounds = child.CalculateBounds(childParentBounds);

            // Expand child bounds to fill the parent bounds
            childBounds.Width = childParentBounds.Width;

            // Add the child drawables
            child.BuildDrawables(drawables, childBounds);

            // Add the spacing
            y += childBounds.Height + spacing;
        }
    }
}