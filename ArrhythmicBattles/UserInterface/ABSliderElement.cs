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

    private readonly RectEntity elementBackgroundEntity = new()
    {
        Radius = 8.0f
    };

    private readonly RectEntity sliderBackgroundEntity = new()
    {
        Radius = 4.0f,
        Color = new Color4(0.0f, 0.0f, 0.0f, 0.4f)
    };

    private readonly RectEntity sliderEntity = new()
    {
        Radius = 4.0f,
        Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)
    };

    private readonly TextEntity textEntity;
    
    private ScopedInputProvider inputProvider;
    private ScopedInputProvider? focusedInputProvider;
    
    private readonly Tweener tweener = new Tweener();
    private bool initialized = false;
    
    private Box2 borderBox;
    private Box2 contentBox;

    public ABSliderElement(Font font, IInputProvider inputProvider, string text)
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
        var from = new Box2(borderBox.Min.X, borderBox.Min.Y, borderBox.Min.X, borderBox.Max.Y);
        var to = borderBox;
        
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
        
        var from = borderBox;
        var to = new Box2(borderBox.Max.X, borderBox.Min.Y, borderBox.Max.X, borderBox.Max.Y);
        
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
            
            elementBackgroundEntity.Min = borderBox.Min;
            elementBackgroundEntity.Max = borderBox.Max;
        }

        interactivity.Update();
        tweener.Update(args.DeltaTime);

        if (interactivity.MouseOver)
        {
            var scrollDelta = inputProvider.MouseScrollDelta;
            
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

    protected override void UpdateLayout(Box2 bounds)
    {
        borderBox = bounds;
        contentBox = new Box2(bounds.Min + new Vector2(16.0f), bounds.Max - new Vector2(16.0f)); // Shrink by 16px on each side
        
        interactivity.Bounds = borderBox;

        sliderBackgroundEntity.Min = new Vector2(contentBox.Max.X - SliderWidth, contentBox.Min.Y);
        sliderBackgroundEntity.Max = contentBox.Max;
        
        ResizeSlider(Value);
    }

    private void ResizeSlider(float value)
    {
        var scrubberBounds = new Box2(
            contentBox.Max.X - SliderWidth + 4.0f, 
            contentBox.Min.Y + 4.0f, 
            contentBox.Max.X - 4.0f, 
            contentBox.Max.Y - 4.0f);
        
        float x = MathHelper.Lerp(scrubberBounds.Min.X, scrubberBounds.Max.X, value);
        sliderEntity.Min = scrubberBounds.Min;
        sliderEntity.Max = new Vector2(x, scrubberBounds.Max.Y);
    }

    public override void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;

        elementBackgroundEntity.Render(args);

        matrixStack.Push();
        matrixStack.Translate(contentBox.Min.X, contentBox.Min.Y, 0.0f);
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