using System.IO.Pipes;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

namespace ArrhythmicBattles.UserInterface;

public class ABSliderElement : VisualElement, IUpdateable, IDisposable
{
    private const float SliderWidth = 256.0f;
    
    public Color4 BackgroundDefaultColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 0.0f);
    public Color4 BackgroundHoverColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 BackgroundPressedColor { get; set; } = new Color4(0.7f, 0.7f, 0.7f, 1.0f);
    public Color4 DefaultColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 HoverColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
    public Color4 PressedColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1.0f);

    public Color4 SliderColor
    {
        get => sliderBackgroundEntity.Color;
        set => sliderBackgroundEntity.Color = value;
    }

    public Color4 SliderScrubberColor
    {
        get => sliderScrubberEntity.Color;
        set => sliderScrubberEntity.Color = value;
    }

    public float Value
    {
        get => value;
        set
        {
            this.value = Math.Clamp(value, 0.0f, 1.0f);
            UpdateScrubber(this.value);
            ValueChanged?.Invoke(this.value);
        }
    }
    
    public Action<float>? ValueChanged { get; set; }

    private float value = 0.0f;

    private readonly FlexFrameworkMain engine;
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

    private readonly RectEntity sliderScrubberEntity = new RectEntity()
    {
        Radius = 4.0f,
        Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)
    };

    private readonly TextEntity textEntity;
    
    private ScopedInputProvider inputProvider;
    private ScopedInputProvider? focusedInputProvider;

    private SimpleAnimator<Color4> backgroundColorAnimator = null!;
    private SimpleAnimator<Color4> colorAnimator = null!;
    private bool initialized = false;

    public ABSliderElement(FlexFrameworkMain engine, IInputProvider inputProvider, string text,
        params Element[] children) : base(children)
    {
        this.engine = engine;
        this.inputProvider = (ScopedInputProvider) inputProvider;

        interactivity = new Interactivity(inputProvider);
        interactivity.MouseButtonUp += OnMouseButtonUp;

        Font font = engine.TextResources.GetFont("inconsolata-regular");

        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
        textEntity.Text = text;
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
            
            LerpFunc<Color4> colorLerpFunc = (left, right, factor) => new Color4(
                MathHelper.Lerp(left.R, right.R, factor),
                MathHelper.Lerp(left.G, right.G, factor),
                MathHelper.Lerp(left.B, right.B, factor),
                MathHelper.Lerp(left.A, right.A, factor));
        
            backgroundColorAnimator = new SimpleAnimator<Color4>(colorLerpFunc, res => elementBackgroundEntity.Color = res, BackgroundDefaultColor, 5.0f);
            colorAnimator = new SimpleAnimator<Color4>(colorLerpFunc, res => textEntity.Color = res, DefaultColor, 5.0f);
        }

        interactivity.Update();
        backgroundColorAnimator.Update(args);
        colorAnimator.Update(args);

        if (interactivity.MouseOver)
        {
            Vector2 scrollDelta = inputProvider.MouseScrollDelta;
            
            if (scrollDelta.Y != 0.0f)
            {
                Value += scrollDelta.Y * 0.01f;
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
            }
        }

        Color4 backgroundColor;
        Color4 color;
        if (focusedInputProvider != null || interactivity.MouseButtons[(int) MouseButton.Left])
        {
            backgroundColor = BackgroundPressedColor;
            color = PressedColor;
        }
        else if (interactivity.MouseOver)
        {
            backgroundColor = BackgroundHoverColor;
            color = HoverColor;
        }
        else
        {
            backgroundColor = BackgroundDefaultColor;
            color = DefaultColor;
        }

        backgroundColorAnimator.LerpTo(backgroundColor);
        colorAnimator.LerpTo(color);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);

        interactivity.Bounds = ElementBounds;
        elementBackgroundEntity.Min = ElementBounds.Min;
        elementBackgroundEntity.Max = ElementBounds.Max;
        
        sliderBackgroundEntity.Min = new Vector2(ContentBounds.X1 - SliderWidth, ContentBounds.Y0);
        sliderBackgroundEntity.Max = ContentBounds.Max;
        
        UpdateScrubber(Value);
    }

    private void UpdateScrubber(float value)
    {
        Bounds scrubberBounds = new Bounds(
            ContentBounds.X1 - SliderWidth + 4.0f, 
            ContentBounds.Y0 + 4.0f, 
            ContentBounds.X1 - 4.0f, 
            ContentBounds.Y1 - 4.0f);
        
        float x = MathHelper.Lerp(scrubberBounds.X0, scrubberBounds.X1, value);
        sliderScrubberEntity.Min = scrubberBounds.Min;
        sliderScrubberEntity.Max = new Vector2(x, scrubberBounds.Y1);
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
            sliderScrubberEntity.Render(args);
        }
    }

    public void Dispose()
    {
        focusedInputProvider?.Dispose();
        elementBackgroundEntity.Dispose();
        sliderBackgroundEntity.Dispose();
        sliderScrubberEntity.Dispose();
        textEntity.Dispose();
    }
}