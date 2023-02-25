using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class StackLayoutElement : Element
{
    public Length Spacing { get; set; } = Length.Zero;

    public StackLayoutElement(params Element[] children)
    {
        Children.AddRange(children);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);

        float spacing = Spacing.Calculate(ContentBounds.Height);

        // Create child drawables
        float y = ContentBounds.Y0;
        foreach (Element child in Children)
        {
            // The child bounds are constrained to the parent bounds
            Bounds childConstraintBounds = new Bounds(ContentBounds.X0, y, ContentBounds.X1, ContentBounds.Y1);

            // Calculate the child bounds
            Bounds childBounds = child.CalculateBoundingBox(childConstraintBounds);
            y += childBounds.Height + spacing; // Add the spacing to the y position

            // Add the child drawables
            child.UpdateLayout(childConstraintBounds);
        }
    }
}