using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class StackLayout : Element
{
    public Length Spacing { get; set; } = Length.Zero;

    public StackLayout(params Element[] children)
    {
        Children.AddRange(children);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        CalculateBounds(constraintBounds, out _, out _, out Bounds contentBounds);

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
            child.UpdateLayout(childConstraintBounds);
        }
    }
}