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
    private const int SliderWidth = 256;
    
    public Color4 BackgroundDefaultColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 0.0f);
    public Color4 BackgroundHoverColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 BackgroundPressedColor { get; set; } = new Color4(0.7f, 0.7f, 0.7f, 1.0f);
    public Color4 DefaultColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 HoverColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
    public Color4 PressedColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1.0f);

    private readonly FlexFrameworkMain engine;
    private readonly Interactivity interactivity;

    private readonly RectEntity elementBackgroundEntity = new RectEntity()
    {
        Radius = 8.0f
    };
    
    private readonly RectEntity sliderBackgroundEntity = new RectEntity()
    {
        Radius = 4.0f
    };
    
    private readonly RectEntity sliderScrubberEntity = new RectEntity()
    {
        Radius = 4.0f
    };

    private readonly TextEntity textEntity;

    private SimpleAnimator<Color4> backgroundColorAnimator = null!;
    private SimpleAnimator<Color4> colorAnimator = null!;
    private bool initialized = false;

    public ABSliderElement(FlexFrameworkMain engine, IInputProvider inputProvider, string text,
        params Element[] children) : base(children)
    {
        this.engine = engine;

        interactivity = new Interactivity(inputProvider);

        Font font = engine.TextResources.GetFont("inconsolata-regular");

        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
        textEntity.Text = text;
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

        Color4 backgroundColor;
        Color4 textColor;
        if (interactivity.MouseButtons[(int) MouseButton.Left])
        {
            backgroundColor = BackgroundPressedColor;
            textColor = PressedColor;
        }
        else if (interactivity.MouseOver)
        {
            backgroundColor = BackgroundHoverColor;
            textColor = HoverColor;
        }
        else
        {
            backgroundColor = BackgroundDefaultColor;
            textColor = DefaultColor;
        }

        backgroundColorAnimator.LerpTo(backgroundColor);
        colorAnimator.LerpTo(textColor);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);

        interactivity.Bounds = ElementBounds;
        elementBackgroundEntity.Min = ElementBounds.Min;
        elementBackgroundEntity.Max = ElementBounds.Max;
    }

    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;

        elementBackgroundEntity.Render(args);

        matrixStack.Push();
        matrixStack.Translate(ContentBounds.X0, ContentBounds.Y0, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        elementBackgroundEntity.Dispose();
        sliderBackgroundEntity.Dispose();
        sliderScrubberEntity.Dispose();
        textEntity.Dispose();
    }
}