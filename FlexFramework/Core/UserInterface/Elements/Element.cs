using System.Collections;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class Element : IEnumerable<Element>
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
    
    protected Element(params Element[] children)
    {
        Children.AddRange(children);
    }

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

    public abstract void UpdateLayout(Bounds constraintBounds);
    
    protected void UpdateChildrenLayout(Bounds contentBounds)
    {
        float y = contentBounds.Y0;
        
        // Render children
        foreach (Element child in Children)
        {
            Bounds childConstraintBounds = new Bounds(contentBounds.X0, y, contentBounds.X1, contentBounds.Y1);
            Bounds childBounds = child.CalculateBoundingBox(childConstraintBounds);
            y += childBounds.Height;

            child.UpdateLayout(childConstraintBounds);
        }
    }

    /// <summary>
    /// Enumerates all elements of the tree, including this element.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Element> GetEnumerator()
    {
        Stack<Element> stack = new Stack<Element>();
        stack.Push(this);
        
        while (stack.Count > 0)
        {
            Element element = stack.Pop();
            yield return element;
            
            int childCount = element.Children.Count;
            for (int i = childCount - 1; i >= 0; i--)
            {
                stack.Push(element.Children[i]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    // Helper methods
    public void UpdateRecursive(UpdateArgs args)
    {
        foreach (IUpdateable updateable in this.OfType<IUpdateable>())
        {
            updateable.Update(args);
        }
    }

    public void RenderRecursive(RenderArgs args)
    {
        foreach (IRenderable renderable in this.OfType<IRenderable>())
        {
            renderable.Render(args);
        }
    }
}