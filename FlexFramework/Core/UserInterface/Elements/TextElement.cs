using FlexFramework.Core.UserInterface.Drawables;
using OpenTK.Mathematics;
using Textwriter;

namespace FlexFramework.Core.UserInterface.Elements;

public class TextElement : EmptyElement
{
    private readonly string text;
    private readonly Color4 color;
    private readonly Font font;

    public TextElement(string text, Color4 color, Font font, params Element[] children) : base(children)
    {
        this.text = text;
        this.color = color;
        this.font = font;
    }
    
    public override void UpdateLayout(Bounds constraintBounds)
    {
        CalculateBounds(constraintBounds, out _, out Bounds elementBounds, out _);
        // drawables.Add(new TextDrawable(engine, elementBounds, text, color, font));
        
        base.UpdateLayout(constraintBounds);
    }
}