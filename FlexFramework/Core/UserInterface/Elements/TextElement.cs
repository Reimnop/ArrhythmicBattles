using FlexFramework.Core.Entities;
using FlexFramework.Text;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class TextElement : VisualElement, IRenderable
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
    
    private Box2 bounds;

    private readonly TextEntity textEntity;

    public TextElement(Font font)
    {
        textEntity = new TextEntity(font);
        textEntity.BaselineOffset = font.Metrics.Height;
    }

    protected override void UpdateLayout(Box2 bounds)
    {
        this.bounds = bounds;
    }

    public override void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(bounds.Min.X, bounds.Min.Y, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }
}