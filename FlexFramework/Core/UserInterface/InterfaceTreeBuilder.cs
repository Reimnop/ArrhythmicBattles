using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Util;

namespace FlexFramework.Core.UserInterface;

public class InterfaceTreeBuilder
{
    private ElementContainer element = new(new EmptyElement());
    private readonly List<InterfaceTreeBuilder> children = new();

    public InterfaceTreeBuilder SetElement(Element element)
    {
        this.element = new ElementContainer(element);
        return this;
    }
    
    public InterfaceTreeBuilder AddChild(InterfaceTreeBuilder child)
    {
        children.Add(child);
        return this;
    }

    public InterfaceTreeBuilder SetPadding(Edges padding)
    {
        element.SetPadding(padding);
        return this;
    }
    
    public InterfaceTreeBuilder SetPadding(float top, float bottom, float left, float right)
    {
        element.SetPadding(top, bottom, left, right);
        return this;
    }
    
    public InterfaceTreeBuilder SetPadding(float value)
    {
        element.SetPadding(value);
        return this;
    }
    
    public InterfaceTreeBuilder SetMargin(Edges margin)
    {
        element.SetMargin(margin);
        return this;
    }
    
    public InterfaceTreeBuilder SetMargin(float top, float bottom, float left, float right)
    {
        element.SetMargin(top, bottom, left, right);
        return this;
    }
    
    public InterfaceTreeBuilder SetMargin(float value)
    {
        element.SetMargin(value);
        return this;
    }
    
    public InterfaceTreeBuilder SetWidth(float width)
    {
        element.SetWidth(width);
        return this;
    }

    public InterfaceTreeBuilder SetWidth(StretchMode mode)
    {
        element.SetWidth(mode);
        return this;
    }
    
    public InterfaceTreeBuilder SetHeight(float height)
    {
        element.SetHeight(height);
        return this;
    }
    
    public InterfaceTreeBuilder SetHeight(StretchMode mode)
    {
        element.SetHeight(mode);
        return this;
    }

    public Node<ElementContainer> Build()
    {
        return new Node<ElementContainer>(element, children.Select(x => x.Build()));
    }
}