using System.Diagnostics;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

public class LayoutEngine
{
    private readonly Node<ElementContainer> root;
    
    public LayoutEngine(Node<ElementContainer> root)
    {
        this.root = root;
    }
    
    public void Layout(Box2 bounds)
    {
        LayoutRecursively(root, bounds);
    }

    private Box2 LayoutRecursively(Node<ElementContainer> node, Box2 parentContentBox)
    {
        // Layout process:
        //
        // 1. Calculate element sizes
        // 2. Calculate element boxes
        // 3. Layout children using element content box
        // 4. Recalculate element boxes based on children bounds
        // 5. Layout children again using new element content box
        // 6. Return element margin box
        
        // Calculate element sizes
        var elementContainer = node.Value;
        var element = elementContainer.Element;
        var elementContentFitSize = element.Size;
        var marginBoxSize = new Vector2(
            node.Value.Width(elementContentFitSize.X, parentContentBox.Size.X),
            node.Value.Height(elementContentFitSize.Y, parentContentBox.Size.Y));
        
        // Calculate element boxes
        var marginBox = new Box2(parentContentBox.Min, parentContentBox.Min + marginBoxSize);
        var borderBox = Shrink(marginBox, elementContainer.Padding);
        var contentBox = Shrink(borderBox, elementContainer.Margin);
        
        // Layout children using element content box
        var childrenBounds = elementContainer.Display(contentBox, node.Children.Select(GetLayoutDelegate));
        
        // Recalculate element boxes based on children bounds
        var fitContentBox = new Box2(
            Math.Min(childrenBounds.Min.X, contentBox.Min.X),
            Math.Min(childrenBounds.Min.Y, contentBox.Min.Y),
            Math.Max(childrenBounds.Max.X, contentBox.Max.X),
            Math.Max(childrenBounds.Max.Y, contentBox.Max.Y));
        var fitPaddingBox = Expand(fitContentBox, elementContainer.Padding);
        var fitMarginBox = Expand(fitPaddingBox, elementContainer.Margin);
        
        marginBoxSize = new Vector2(
            node.Value.Width(fitMarginBox.Size.X, parentContentBox.Size.X),
            node.Value.Height(fitMarginBox.Size.Y, parentContentBox.Size.Y));
        marginBox = new Box2(parentContentBox.Min, parentContentBox.Min + marginBoxSize);
        borderBox = Shrink(marginBox, elementContainer.Padding);
        contentBox = Shrink(borderBox, elementContainer.Margin);
        
        // Layout children again using new element content box
        elementContainer.Display(contentBox, node.Children.Select(GetLayoutDelegate));
        
        // Call element layout callback
        element.LayoutCallback(new ElementBoxes(marginBox, borderBox, contentBox));

        // Return element margin box
        return marginBox;
    }

    private LayoutDelegate GetLayoutDelegate(Node<ElementContainer> node) => bounds => LayoutRecursively(node, bounds);

    private static Box2 Shrink(Box2 box, Edges edges)
    {
        var minX = box.Min.X + edges.Left;
        var minY = box.Min.Y + edges.Top;
        var maxX = box.Max.X - edges.Right;
        var maxY = box.Max.Y - edges.Bottom;
        return new Box2(minX, minY, maxX, maxY);
    }

    private static Box2 Expand(Box2 box, Edges edges)
    {
        var minX = box.Min.X - edges.Left;
        var minY = box.Min.Y - edges.Top;
        var maxX = box.Max.X + edges.Right;
        var maxY = box.Max.Y + edges.Bottom;
        return new Box2(minX, minY, maxX, maxY);
    }
}