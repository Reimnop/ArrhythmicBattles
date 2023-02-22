using FlexFramework.Core.UserInterface.Drawables;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class EmptyElement : Element
{
    public EmptyElement(params Element[] children)
    {
        Children.AddRange(children);
    }
    
    public override void BuildDrawables(List<Drawable> drawables, FlexFrameworkMain engine, Bounds constraintBounds)
    {
        DrawDebugBounds(drawables, engine, constraintBounds);
        CalculateBounds(constraintBounds, out _, out _, out Bounds contentBounds);

        float y = contentBounds.Y0;
        
        // Render children
        foreach (Element child in Children)
        {
            Bounds childConstraintBounds = new Bounds(contentBounds.X0, y, contentBounds.X1, contentBounds.Y1);
            Bounds childBounds = child.CalculateBoundingBox(childConstraintBounds);
            y += childBounds.Height;

            child.BuildDrawables(drawables, engine, childConstraintBounds);
        }
    }
}