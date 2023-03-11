using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using Glide;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

namespace ArrhythmicBattles.UserInterface;

public class ABButtonElement : VisualElement, IUpdateable, IDisposable
{
    public Color4 TextDefaultColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 TextHoverColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
    public Action? Click { get; set; }

    private readonly FlexFrameworkMain engine;
    private readonly Interactivity interactivity;
    
    private readonly RectEntity rectEntity = new RectEntity()
    {
        Radius = 8.0f
    };

    private readonly TextEntity textEntity;
    private readonly Tweener tweener = new Tweener();
    private bool initialized = false;

    public ABButtonElement(FlexFrameworkMain engine, IInputProvider inputProvider, string text, params Element[] children) : base(children)
    {
        this.engine = engine;
        
        interactivity = new Interactivity(inputProvider);
        interactivity.MouseButtonUp += OnMouseButtonUp;
        interactivity.MouseEnter += OnMouseEnter;
        interactivity.MouseLeave += OnMouseLeave;
        
        var textAssetsLocation = engine.DefaultAssets.TextAssets;
        var textAssets = engine.ResourceRegistry.GetResource(textAssetsLocation);
        Font font = textAssets["inconsolata"];

        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
        textEntity.Text = text;
    }

    private void OnMouseEnter()
    {
        Bounds from = new Bounds(ElementBounds.X0, ElementBounds.Y0, ElementBounds.X0, ElementBounds.Y1);
        Bounds to = ElementBounds;
        
        rectEntity.Min = from.Min;
        rectEntity.Max = from.Max;
        tweener.Tween(rectEntity, new {to.Min, to.Max, Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)}, 0.2f).Ease(Easing.QuadInOut);
        tweener.Tween(textEntity, new {Color = TextHoverColor}, 0.2f).Ease(Easing.QuadInOut);
    }
    
    private void OnMouseLeave()
    {
        Bounds from = ElementBounds;
        Bounds to = new Bounds(ElementBounds.X1, ElementBounds.Y0, ElementBounds.X1, ElementBounds.Y1);
        
        rectEntity.Min = from.Min;
        rectEntity.Max = from.Max;
        tweener.Tween(rectEntity, new {to.Min, to.Max, Color = new Color4(1.0f, 1.0f, 1.0f, 0.0f)}, 0.2f).Ease(Easing.QuadInOut);
        tweener.Tween(textEntity, new {Color = TextDefaultColor}, 0.2f).Ease(Easing.QuadInOut);
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

            rectEntity.Min = ElementBounds.Min;
            rectEntity.Max = ElementBounds.Max;
        }
        
        interactivity.Update();
        tweener.Update(args.DeltaTime);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
        
        interactivity.Bounds = ElementBounds;
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

    public void Dispose()
    {
        rectEntity.Dispose();
        textEntity.Dispose();
    }
}