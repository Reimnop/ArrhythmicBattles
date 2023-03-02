using System.Security.AccessControl;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

namespace ArrhythmicBattles.UserInterface;

public class ABButtonElement : VisualElement, IUpdateable, IDisposable
{
    public Color4 TextDefaultColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 TextHoverColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
    public Color4 TextPressedColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
    public Action? Click { get; set; }

    private readonly FlexFrameworkMain engine;
    private readonly Interactivity interactivity;
    
    private readonly RectEntity rectEntity = new RectEntity()
    {
        Radius = 8.0f
    };

    private readonly TextEntity textEntity;
    
    private SimpleAnimator<Bounds> backgroundAnimator = null!;
    private SimpleAnimator<float> backgroundOpacityAnimator = null!;
    private SimpleAnimator<Color4> textColorAnimator = null!;
    private Bounds backgroundBounds = new Bounds();
    private bool initialized = false;

    public ABButtonElement(FlexFrameworkMain engine, IInputProvider inputProvider, string text, params Element[] children) : base(children)
    {
        this.engine = engine;
        
        interactivity = new Interactivity(inputProvider);
        interactivity.MouseButtonUp += OnMouseButtonUp;
        interactivity.MouseEnter += OnMouseEnter;
        interactivity.MouseLeave += OnMouseLeave;

        Font font = engine.TextResources.GetFont("inconsolata-regular");
        
        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
        textEntity.Text = text;
    }

    private void OnMouseEnter()
    {
        Bounds from = new Bounds(backgroundBounds.X0, backgroundBounds.Y0, backgroundBounds.X0, backgroundBounds.Y1);
        Bounds to = backgroundBounds;
        
        backgroundAnimator.LerpFromTo(from, to);
        backgroundOpacityAnimator.LerpTo(1.0f);
    }
    
    private void OnMouseLeave()
    {
        Bounds from = backgroundBounds;
        Bounds to = new Bounds(backgroundBounds.X1, backgroundBounds.Y0, backgroundBounds.X1, backgroundBounds.Y1);
        
        backgroundAnimator.LerpFromTo(from, to);
        backgroundOpacityAnimator.LerpTo(0.0f);
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
            
            LerpFunc<Color4> colorLerpFunc = (left, right, factor) => new Color4(
                MathHelper.Lerp(left.R, right.R, factor), 
                MathHelper.Lerp(left.G, right.G, factor), 
                MathHelper.Lerp(left.B, right.B, factor), 
                MathHelper.Lerp(left.A, right.A, factor));
            
            LerpFunc<Bounds> boundsLerpFunc = (left, right, factor) => new Bounds(
                MathHelper.Lerp(left.X0, right.X0, factor),
                MathHelper.Lerp(left.Y0, right.Y0, factor),
                MathHelper.Lerp(left.X1, right.X1, factor),
                MathHelper.Lerp(left.Y1, right.Y1, factor));

            backgroundAnimator = new SimpleAnimator<Bounds>(
                boundsLerpFunc, 
                res =>
                {
                    rectEntity.Min = res.Min;
                    rectEntity.Max = res.Max;
                },
                new Bounds(backgroundBounds.X0, backgroundBounds.Y0, backgroundBounds.X0, backgroundBounds.Y1),
                5.0f);
            backgroundOpacityAnimator = new SimpleAnimator<float>(
                MathHelper.Lerp, 
                res => rectEntity.Color = new Color4(1.0f, 1.0f, 1.0f, res),
                0.0f,
                5.0f);
            textColorAnimator = new SimpleAnimator<Color4>(colorLerpFunc, res => textEntity.Color = res, TextDefaultColor, 5.0f);
        }
        
        interactivity.Update();
        backgroundAnimator.Update(args);
        backgroundOpacityAnimator.Update(args);
        textColorAnimator.Update(args);
        
        Color4 textColor;
        if (interactivity.MouseButtons[(int) MouseButton.Left])
        {
            textColor = TextPressedColor;
        }
        else if (interactivity.MouseOver)
        {
            textColor = TextHoverColor;
        }
        else
        {
            textColor = TextDefaultColor;
        }
        
        textColorAnimator.LerpTo(textColor);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
        
        interactivity.Bounds = ElementBounds;
        backgroundBounds = ElementBounds;
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