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
    public Color4 BackgroundDefaultColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 0.0f);
    public Color4 BackgroundHoverColor { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 BackgroundPressedColor { get; set; } = new Color4(0.7f, 0.7f, 0.7f, 1.0f);
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
    
    private readonly SimpleAnimator<Color4> backgroundColorAnimator;
    private readonly SimpleAnimator<Color4> textColorAnimator;

    public ABButtonElement(FlexFrameworkMain engine, IInputProvider inputProvider, string text, params Element[] children) : base(children)
    {
        this.engine = engine;
        
        interactivity = new Interactivity(inputProvider);
        interactivity.MouseButtonUp += OnMouseButtonUp;

        LerpFunc<Color4> colorLerpFunc = (left, right, factor) => new Color4(
            MathHelper.Lerp(left.R, right.R, factor), 
            MathHelper.Lerp(left.G, right.G, factor), 
            MathHelper.Lerp(left.B, right.B, factor), 
            MathHelper.Lerp(left.A, right.A, factor));
        
        Font font = engine.TextResources.GetFont("inconsolata-regular");
        
        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
        textEntity.Text = text;

        backgroundColorAnimator = new SimpleAnimator<Color4>(colorLerpFunc,res => rectEntity.Color = res, BackgroundDefaultColor, 5.0f);
        textColorAnimator = new SimpleAnimator<Color4>(colorLerpFunc, res => textEntity.Color = res, TextDefaultColor, 5.0f);
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
        interactivity.Update();
        backgroundColorAnimator.Update(args);
        textColorAnimator.Update(args);

        Color4 backgroundColor;
        Color4 textColor;
        if (interactivity.MouseButtons[(int) MouseButton.Left])
        {
            backgroundColor = BackgroundPressedColor;
            textColor = TextPressedColor;
        }
        else if (interactivity.MouseOver)
        {
            backgroundColor = BackgroundHoverColor;
            textColor = TextHoverColor;
        }
        else
        {
            backgroundColor = BackgroundDefaultColor;
            textColor = TextDefaultColor;
        }
        
        backgroundColorAnimator.LerpTo(backgroundColor);
        textColorAnimator.LerpTo(textColor);
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
        
        interactivity.Bounds = ElementBounds;
        rectEntity.Min = ElementBounds.Min;
        rectEntity.Max = ElementBounds.Max;
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