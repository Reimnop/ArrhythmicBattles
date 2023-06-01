using FlexFramework.Core.Entities;
using FlexFramework.Text;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class TextElement : VisualElement, IRenderable
{
    public override Vector2 Size => textBounds != null
        ? new Vector2(textBounds.MaxX - textBounds.MinX, textBounds.MaxY - textBounds.MinY) / 64.0f
        : Vector2.Zero;
    
    public string Text
    {
        get => textEntity.Text;
        set
        {
            textEntity.Text = value;
            textBounds = TextShaper.GetTextBounds(
                textEntity.Font, 
                value, 
                textEntity.HorizontalAlignment,
                textEntity.VerticalAlignment);
        }
    }

    public Color4 Color
    {
        get => textEntity.Color;
        set => textEntity.Color = value;
    }
    
    private Box2 contentBox;

    private readonly TextEntity textEntity;
    private TextBounds? textBounds;

    public TextElement(Font font)
    {
        textEntity = new TextEntity(font);
        textEntity.BaselineOffset = font.Metrics.Height;
    }

    public override void LayoutCallback(ElementBoxes boxes)
    {
        contentBox = boxes.ContentBox;
    }

    public override void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(contentBox.Min.X, contentBox.Min.Y, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }
}