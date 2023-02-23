using FlexFramework.Core.UserInterface.Drawables;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class RectElement : EmptyElement
{
    public float Radius { get; set; } = 0.0f;
    public Color4 Color { get; set; } = Color4.White;
    
    public RectElement(params Element[] children) : base(children)
    {
    }
}