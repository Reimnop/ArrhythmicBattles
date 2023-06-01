using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

public delegate Box2 LayoutDelegate(Box2 bounds);
public delegate Box2 DisplayDelegate(Box2 parentBounds, IEnumerable<LayoutDelegate> childLayoutDelegates); // fauxtional programming moment
public delegate float DimensionDelegate(float fit, float parent);

public class ElementContainer
{
    public Element Element { get; }
    public DimensionDelegate Width { get; set; } = GetDimensionDelegate(StretchMode.Fit);
    public DimensionDelegate Height { get; set; } = GetDimensionDelegate(StretchMode.Fit);
    public DisplayDelegate Display { get; set; } = GetDisplayDelegate(DisplayMode.Block);
    public Edges Padding { get; set; }
    public Edges Margin { get; set; }
    
    public ElementContainer(Element element)
    {
        Element = element;
    }
    
    public ElementContainer SetPadding(Edges padding)
    {
        Padding = padding;
        return this;
    }
    
    public ElementContainer SetPadding(float top, float bottom, float left, float right)
    {
        return SetPadding(new Edges(top, bottom, left, right));
    }
    
    public ElementContainer SetPadding(float value)
    {
        return SetPadding(new Edges(value));
    }
    
    public ElementContainer SetMargin(Edges margin)
    {
        Margin = margin;
        return this;
    }
    
    public ElementContainer SetMargin(float top, float bottom, float left, float right)
    {
        return SetMargin(new Edges(top, bottom, left, right));
    }
    
    public ElementContainer SetMargin(float value)
    {
        return SetMargin(new Edges(value));
    }
    
    public ElementContainer SetWidth(float width)
    {
        Width = (_, _) => width;
        return this;
    }

    public ElementContainer SetWidth(StretchMode mode)
    {
        Width = GetDimensionDelegate(mode);
        return this;
    }
    
    public ElementContainer SetHeight(float height)
    {
        Height = (_, _) => height;
        return this;
    }
    
    public ElementContainer SetHeight(StretchMode mode)
    {
        Height = GetDimensionDelegate(mode);
        return this;
    }
    
    public ElementContainer SetDisplayMode(DisplayMode mode)
    {
        Display = GetDisplayDelegate(mode);
        return this;
    }

    private static DimensionDelegate GetDimensionDelegate(StretchMode mode)
    {
        return mode switch
        {
            StretchMode.Fit => FitStretchMode,
            StretchMode.Stretch => StretchStretchMode,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    private static float FitStretchMode(float fit, float parent) => fit;
    private static float StretchStretchMode(float fit, float parent) => parent;
    
    private static DisplayDelegate GetDisplayDelegate(DisplayMode mode)
    {
        return mode switch
        {
            DisplayMode.Block => BlockDisplayMode,
            DisplayMode.Inline => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    private static Box2 BlockDisplayMode(Box2 parentBounds, IEnumerable<LayoutDelegate> childLayoutDelegates)
    {
        var currentBounds = new Box2(parentBounds.Min, parentBounds.Min);
        var currentParentBounds = parentBounds;
        foreach (var childLayoutDelegate in childLayoutDelegates)
        {
            var childBounds = childLayoutDelegate(currentParentBounds);
            currentParentBounds.Translate(Vector2.UnitY * childBounds.Size.Y);
            currentBounds.Inflate(childBounds.Min);
            currentBounds.Inflate(childBounds.Max);
        }
        return currentBounds;
    }
}