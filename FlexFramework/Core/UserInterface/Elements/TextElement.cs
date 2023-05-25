using FlexFramework.Core.Entities;
using FlexFramework.Text;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class TextElement : VisualElement, IRenderable
{
    public string Text
    {
        get => textEntity.Text;
        set
        {
            textEntity.Text = value;

            if (autoHeight)
            {
                var lines = value.Split('\n').Length;
                var height = lines * (font.Metrics.Height >> 6) * textEntity.EmSize;
                Height = height;
            }
        }
    }

    public Color4 Color
    {
        get => textEntity.Color;
        set => textEntity.Color = value;
    }

    private readonly TextEntity textEntity;
    private readonly Font font;
    private readonly bool autoHeight;

    public TextElement(Font font, bool autoHeight = true, params Element[] children) : base(children)
    {
        this.autoHeight = autoHeight;
        this.font = font;

        textEntity = new TextEntity(font);
        textEntity.BaselineOffset = font.Metrics.Height;
    }
    
    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
    }

    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(ElementBounds.X0, ElementBounds.Y0, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }
}