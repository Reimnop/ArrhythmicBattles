using ArrhythmicBattles.Core;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Text;
using Glide;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.UserInterface;

public class ABSliderElement : VisualElement, IUpdateable, IDisposable
{
    private const float SliderWidth = 256.0f;
    
    public Color4 DefaultColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 HoverColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1.0f);

    public Color4 SliderColor
    {
        get => sliderBackgroundEntity.Color;
        set => sliderBackgroundEntity.Color = value;
    }

    public Color4 SliderScrubberColor
    {
        get => sliderEntity.Color;
        set => sliderEntity.Color = value;
    }

    public float Value
    {
        get => value;
        set
        {
            this.value = Math.Clamp(value, 0.0f, 1.0f);
            ValueChanged?.Invoke(this.value);
            ResizeSlider(this.value);
        }
    }

    public Action<float>? ValueChanged { get; set; }

    private float value = 0.0f;
    private readonly Interactivity interactivity;

    private readonly RectEntity elementBackgroundEntity = new RectEntity()
    {
        Radius = 8.0f
    };

    private readonly RectEntity sliderBackgroundEntity = new RectEntity()
    {
        Radius = 4.0f,
        Color = new Color4(0.0f, 0.0f, 0.0f, 0.4f)
    };

    private readonly RectEntity sliderEntity = new RectEntity()
    {
        Radius = 4.0f,
        Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)
    };

    private readonly TextEntity textEntity;
    
    private ScopedInputProvider inputProvider;
    private ScopedInputProvider? focusedInputProvider;
    
    private readonly Tweener tweener = new Tweener();
    private bool initialized = false;

    public ABSliderElement(Font font, IInputProvider inputProvider, string text, params Element[] children) : base(children)
    {
        this.inputProvider = (ScopedInputProvider) inputProvider;

        interactivity = new Interactivity(inputProvider);
        interactivity.MouseButtonUp += OnMouseButtonUp;
        interactivity.MouseEnter += AnimateHighlight;
        interactivity.MouseLeave += AnimateUnhighlight;

        textEntity = new TextEntity(font);
        textEntity.BaselineOffset = font.Metrics.Height;
        textEntity.Text = text;
    }
    
    private void AnimateHighlight()
    {
        Bounds from = new Bounds(Bounds.X0, Bounds.Y0, Bounds.X0, Bounds.Y1);
        Bounds to = Bounds;
        
        elementBackgroundEntity.Min = from.Min;
        elementBackgroundEntity.Max = from.Max;
        tweener.Tween(elementBackgroundEntity, new {to.Min, to.Max, Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)}, 0.2f).Ease(Ease.QuadInOut);
        tweener.Tween(textEntity, new {Color = new Color4(0.0f, 0.0f, 0.0f, 1.0f)}, 0.2f).Ease(Ease.QuadInOut);
    }
    
    private void AnimateUnhighlight()
    {
        if (focusedInputProvider != null)
        {
            return;
        }
        
        Bounds from = Bounds;
        Bounds to = new Bounds(Bounds.X1, Bounds.Y0, Bounds.X1, Bounds.Y1);
        
        elementBackgroundEntity.Min = from.Min;
        elementBackgroundEntity.Max = from.Max;
        tweener.Tween(elementBackgroundEntity, new {to.Min, to.Max, Color = new Color4(1.0f, 1.0f, 1.0f, 0.0f)}, 0.2f).Ease(Ease.QuadInOut);
        tweener.Tween(textEntity, new {Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)}, 0.2f).Ease(Ease.QuadInOut);
    }

    private void OnMouseButtonUp(MouseButton button)
    {
        if (button == MouseButton.Left)
        {
            focusedInputProvider = inputProvider.InputSystem.AcquireInputProvider();
        }
    }

    public void Update(UpdateArgs args)
    {
        if (!initialized)
        {
            initialized = true;
            
            elementBackgroundEntity.Color = new Color4(0.0f, 0.0f, 0.0f, 0.0f);
            textEntity.Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
            
            elementBackgroundEntity.Min = Bounds.Min;
            elementBackgroundEntity.Max = Bounds.Max;
        }

        interactivity.Update();
        tweener.Update(args.DeltaTime);

        if (interactivity.MouseOver)
        {
            Vector2 scrollDelta = inputProvider.MouseScrollDelta;
            
            if (scrollDelta.Y != 0.0f)
            {
                Value += scrollDelta.Y * 0.02f;
            }
        }

        if (focusedInputProvider != null)
        {
            if (focusedInputProvider.GetKeyDown(Keys.Left))
            {
                Value -= 0.1f;
            }
            else if (focusedInputProvider.GetKeyDown(Keys.Right))
            {
                Value += 0.1f;
            }

            if (focusedInputProvider.GetKeyDown(Keys.Enter))
            {
                focusedInputProvider.Dispose();
                focusedInputProvider = null;
                AnimateUnhighlight();
            }
        }
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);

        interactivity.Bounds = Bounds;

        sliderBackgroundEntity.Min = new Vector2(ContentBounds.X1 - SliderWidth, ContentBounds.Y0);
        sliderBackgroundEntity.Max = ContentBounds.Max;
        
        ResizeSlider(Value);
    }

    private void ResizeSlider(float value)
    {
        Bounds scrubberBounds = new Bounds(
            ContentBounds.X1 - SliderWidth + 4.0f, 
            ContentBounds.Y0 + 4.0f, 
            ContentBounds.X1 - 4.0f, 
            ContentBounds.Y1 - 4.0f);
        
        float x = MathHelper.Lerp(scrubberBounds.X0, scrubberBounds.X1, value);
        sliderEntity.Min = scrubberBounds.Min;
        sliderEntity.Max = new Vector2(x, scrubberBounds.Y1);
    }

    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;

        elementBackgroundEntity.Render(args);

        matrixStack.Push();
        matrixStack.Translate(ContentBounds.X0, ContentBounds.Y0, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
        
        sliderBackgroundEntity.Render(args);

        if (Value > 0.0f)
        {
            sliderEntity.Render(args);
        }
    }

    public void Dispose()
    {
        focusedInputProvider?.Dispose();
    }
}