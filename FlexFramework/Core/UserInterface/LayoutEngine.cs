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
        var rootBox = new ElementBox(bounds, bounds, bounds);
        LayoutRecursively(root, rootBox);
    }

    private Box2 LayoutRecursively(Node<ElementContainer> node, ElementBox parentBox)
    {
        // Get children bounds
        Box2 childrenBox = default;
        var childOffsetY = 0.0f;
        foreach (var child in node.Children)
        {
            var childParentBox = parentBox.Translate(new Vector2(0.0f, childOffsetY));
            var childBox = LayoutRecursively(child, childParentBox);
            childOffsetY += childBox.Size.Y;
            childrenBox = ExpandToContain(childrenBox, childBox);
        }
        
        // Get element bounds
        var element = node.Value.Element;
        var elementSize = element.Size;
        
        // Calculate boxes
        var marginBox = parentBox.ContentBox;
        var borderBox = Shrink(marginBox, node.Value.Margin);
        var contentBox = Shrink(borderBox, node.Value.Padding);
        
        // Calculate actual content size
        var fitWidth = Math.Max(elementSize.X, childrenBox.Size.X);
        var fitHeight = Math.Max(elementSize.Y, childrenBox.Size.Y);
        var actualWidth = node.Value.Width(fitWidth, contentBox.Size.X);
        var actualHeight = node.Value.Height(fitHeight, contentBox.Size.Y);
        var actualSize = new Vector2(actualWidth, actualHeight);

        if (contentBox.Size != actualSize)
        {
            // Recalculate boxes
            contentBox = new Box2(contentBox.Min, contentBox.Min + actualSize);
            borderBox = Expand(contentBox, node.Value.Padding);
            marginBox = Expand(borderBox, node.Value.Margin);
        }
        
        // Set final box
        var box = new ElementBox(marginBox, borderBox, contentBox);
        element.SetBox(box);

        return marginBox;
    }

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

    private static Box2 ExpandToContain(Box2 box, Box2 expandTo)
    {
        var minX = Math.Min(box.Min.X, expandTo.Min.X);
        var minY = Math.Min(box.Min.Y, expandTo.Min.Y);
        var maxX = Math.Max(box.Max.X, expandTo.Max.X);
        var maxY = Math.Max(box.Max.Y, expandTo.Max.Y);
        return new Box2(minX, minY, maxX, maxY);
    }
}