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

        textEntity = new TextEntity(font)
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private void OnMouseEnter()
    {
        var from = new Box2(borderBox.Min.X, borderBox.Min.Y, borderBox.Min.X, borderBox.Max.Y);
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

    protected override void UpdateLayout(Box2 bounds)
    {
        borderBox = bounds;
        contentBox = new Box2(borderBox.Min + new Vector2(16.0f), borderBox.Max - new Vector2(16.0f)); // Shrink by 16px on each side

        interactivity.Bounds = borderBox;
        textEntity.Bounds = contentBox;
    }

    public override void Render(RenderArgs args)
    {
        rectEntity.Render(args);
        textEntity.Render(args);
    }
}