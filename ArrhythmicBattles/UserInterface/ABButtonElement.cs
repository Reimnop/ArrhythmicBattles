using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Text;
using Glide;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.UserInterface;

public class ABButtonElement : VisualElement, IUpdateable
{
    // TODO: Change width to be dynamic based on text width
    public override Vector2 Size => new(0.0f, 64.0f);
    
    public Color4 TextDefaultColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 TextHoverColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
    public Action? Click { get; set; }
    
    private readonly Interactivity interactivity;
    
    private readonly RectEntity rectEntity = new RectEntity()
    {
        Radius = 8.0f
    };

    private readonly TextEntity textEntity;
    private readonly Tweener tweener = new Tweener();
    private bool initialized = false;
    
    private Box2 borderBox;
    private Box2 contentBox;

    public ABButtonElement(Font font, IInputProvider inputProvider, string text)
    {
        interactivity = new Interactivity(inputProvider);
        interactivity.MouseButtonUp += OnMouseButtonUp;
        interactivity.MouseEnter += OnMouseEnter;
        interactivity.MouseLeave += OnMouseLeave;

        textEntity = new TextEntity(font);
        textEntity.BaselineOffset = font.Metrics.Height;
        textEntity.Text = text;
    }

    private void OnMouseEnter()
    {
        var from = new Box2(borderBox.Min.X, borderBox.Min.Y, borderBox.Min.X, borderBox.Max.X);
        var to = borderBox;
        
        rectEntity.Min = from.Min;
        rectEntity.Max = from.Max;
        tweener.Tween(rectEntity, new {to.Min, to.Max, Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)}, 0.2f).Ease(Ease.QuadInOut);
        tweener.Tween(textEntity, new {Color = TextHoverColor}, 0.2f).Ease(Ease.QuadInOut);
    }
    
    private void OnMouseLeave()
    {
        var from = borderBox;
        var to = new Box2(borderBox.Max.X, borderBox.Min.Y, borderBox.Max.X, borderBox.Max.Y);
        
        rectEntity.Min = from.Min;
        rectEntity.Max = from.Max;
        tweener.Tween(rectEntity, new {to.Min, to.Max, Color = new Color4(1.0f, 1.0f, 1.0f, 0.0f)}, 0.2f).Ease(Ease.QuadInOut);
        tweener.Tween(textEntity, new {Color = TextDefaultColor}, 0.2f).Ease(Ease.QuadInOut);
    }

    private void OnMouseButtonUp(MouseButton button)
    {
        if (button == MouseButton.Left)
        {
            Click?.Invoke();
        }
    }

    public void Update(UpdateArgs args)
    {
        if (!initialized)
        {
            initialized = true;
            
            rectEntity.Color = new Color4(0.0f, 0.0f, 0.0f, 0.0f);
            textEntity.Color = TextDefaultColor;

            rectEntity.Min = borderBox.Min;
            rectEntity.Max = borderBox.Max;
        }
        
        interactivity.Update();
        tweener.Update(args.DeltaTime);
    }

    public override void LayoutCallback(ElementBoxes boxes)
    {
        borderBox = boxes.BorderBox;
        contentBox = boxes.ContentBox;

        interactivity.Bounds = boxes.BorderBox;
    }

    public override void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        rectEntity.Render(args);
        
        matrixStack.Push();
        matrixStack.Translate(contentBox.Min.X, contentBox.Min.Y, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }
}