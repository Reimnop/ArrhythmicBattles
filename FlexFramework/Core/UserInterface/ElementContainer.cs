using FlexFramework.Core.UserInterface.Elements;

namespace FlexFramework.Core.UserInterface;

public delegate float LayoutDelegate(float fit, float parent);

public class ElementContainer
{
    public Element Element { get; }
    public LayoutDelegate Width { get; set; }
    public LayoutDelegate Height { get; set; }
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
    
    public ElementContainer SetPadding(float left, float top, float right, float bottom)
    {
        return SetPadding(new Edges(left, top, right, bottom));
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
    
    public ElementContainer SetMargin(float left, float top, float right, float bottom)
    {
        return SetMargin(new Edges(left, top, right, bottom));
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
        Width = GetLayoutDelegate(mode);
        return this;
    }
    
    public ElementContainer SetHeight(float height)
    {
        Height = (_, _) => height;
        return this;
    }
    
    public ElementContainer SetHeight(StretchMode mode)
    {
        Height = GetLayoutDelegate(mode);
        return this;
    }

    private LayoutDelegate GetLayoutDelegate(StretchMode mode)
    {
        return mode switch
        {
            StretchMode.Fit => (fit, _) => fit,
            StretchMode.Stretch => (_, parent) => parent,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}