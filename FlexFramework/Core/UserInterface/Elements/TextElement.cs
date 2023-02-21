using FlexFramework.Core.UserInterface.Renderables;
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
    
    public override void BuildRenderables(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds constraintBounds)
    {
        DrawDebugBounds(renderables, engine, constraintBounds);
        CalculateBounds(constraintBounds, out _, out Bounds elementBounds, out _);
        renderables.Add(new TextRenderable(engine, elementBounds, text, color, font));
        
        base.BuildRenderables(renderables, engine, constraintBounds);
    }
}