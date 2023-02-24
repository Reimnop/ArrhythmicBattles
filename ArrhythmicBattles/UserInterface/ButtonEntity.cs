using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

namespace ArrhythmicBattles.UserInterface;

public class ButtonEntity : UIElement, IRenderable, IDisposable
{
    public override Vector2 Position { get; set; }
    public override Vector2 Size { get; set; }
    public override Vector2 Origin { get; set; }
    public override bool IsFocused { get; set; }

    public string Text
    {
        get => textEntity.Text;
        set => textEntity.Text = value;
    }

    public Color4 TextUnfocusedColor { get; set; } = Color4.White;
    public Color4 TextFocusedColor { get; set; } = Color4.Black;

    public event Action? Pressed;

    public Vector2i TextPosOffset { get; set; }
    
    private readonly IInputProvider inputProvider;
    private readonly TextEntity textEntity;

    private SimpleAnimator<Color4> colorAnimator = null!;

    public ButtonEntity(FlexFrameworkMain engine, IInputProvider inputProvider) : base(engine)
    {
        this.inputProvider = inputProvider;
        
        textEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-regular"));
        textEntity.HorizontalAlignment = HorizontalAlignment.Left;
    }

    public override void Start()
    {
        colorAnimator = new SimpleAnimator<Color4>(
            (left, right, factor) =>
            {
                float t = Easing.QuadInOut(factor);
                return new Color4(
                    MathHelper.Lerp(left.R, right.R, t),
                    MathHelper.Lerp(left.G, right.G, t),
                    MathHelper.Lerp(left.B, right.B, t),
                    MathHelper.Lerp(left.A, right.A, t));
            },
            value => textEntity.Color = value,
            TextUnfocusedColor,
            10.0f);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        colorAnimator.Update(args);

        if (IsFocused && inputProvider.GetKeyDown(Keys.Enter))
        {
            Pressed?.Invoke();
        }
    }

    protected override void OnFocused()
    {
        colorAnimator.LerpTo(TextFocusedColor);
    }

    protected override void OnUnfocused()
    {
        colorAnimator.LerpTo(TextUnfocusedColor);
    }

    public void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0f);

        matrixStack.Push();
        matrixStack.Translate(-Origin.X * Size.X, -Origin.Y * Size.Y, 0.0f);
        matrixStack.Translate(TextPosOffset.X, TextPosOffset.Y, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
        
        matrixStack.Pop();
    }

    public void Dispose()
    {
        textEntity.Dispose();
    }
}