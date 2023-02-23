using FlexFramework.Core.Entities;
using OpenTK.Mathematics;
using Textwriter;

namespace FlexFramework.Core.UserInterface.Elements;

public class TextElement : EmptyElement, IRenderable, IDisposable
{
    public string Text
    {
        get => textEntity.Text;
        set => textEntity.Text = value;
    }

    public Color4 Color
    {
        get => textEntity.Color;
        set => textEntity.Color = value;
    }

    private readonly TextEntity textEntity;
    private Bounds elementBounds;

    public TextElement(FlexFrameworkMain engine, Font font, params Element[] children) : base(children)
    {
        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
    }
    
    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        CalculateBounds(constraintBounds, out _, out elementBounds, out _);
    }

    public void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(elementBounds.X0, elementBounds.Y0, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        textEntity.Dispose();
    }
}