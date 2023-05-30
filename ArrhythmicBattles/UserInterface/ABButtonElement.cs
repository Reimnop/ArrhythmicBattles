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

    public ABButtonElement(Font font, IInputProvider inputProvider, string text, params Element[] children) : base(children)
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
        Bounds from = new Bounds(Bounds.X0, Bounds.Y0, Bounds.X0, Bounds.Y1);
        Bounds to = Bounds;
        
        rectEntity.Min = from.Min;
        rectEntity.Max = from.Max;
        tweener.Tween(rectEntity, new {to.Min, to.Max, Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)}, 0.2f).Ease(Ease.QuadInOut);
        tweener.Tween(textEntity, new {Color = TextHoverColor}, 0.2f).Ease(Ease.QuadInOut);
    }
    
    private void OnMouseLeave()
    {
        Bounds from = Bounds;
        Bounds to = new Bounds(Bounds.X1, Bounds.Y0, Bounds.X1, Bounds.Y1);
        
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

            rectEntity.Min = Bounds.Min;
            rectEntity.Max = Bounds.Max;
        }
        
        interactivity.Update();
        tweener.Update(args.DeltaTime);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
        
        interactivity.Bounds = Bounds;
    }

    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        rectEntity.Render(args);
        
        matrixStack.Push();
        matrixStack.Translate(ContentBounds.X0, ContentBounds.Y0, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }
}