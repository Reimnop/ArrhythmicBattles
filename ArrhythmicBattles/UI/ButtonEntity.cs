using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

namespace ArrhythmicBattles.UI;

public class ButtonEntity : UIElement, IRenderable
{
    public override Vector2i Position { get; set; }
    public override Vector2i Size { get; set; }
    public override Vector2d Origin { get; set; }
    public override bool IsFocused { get; set; }

    public string Text
    {
        get => textEntity.Text;
        set => textEntity.Text = value;
    }

    public Color4 TextUnfocusedColor { get; set; } = Color4.White;
    public Color4 TextFocusedColor { get; set; } = Color4.Black;

    public event Action? PressedCallback;

    public Vector2i TextPosOffset { get; set; }

    private double textPaddingX = 0.0;
    
    private readonly TextEntity textEntity;
    private readonly SimpleAnimator<double> paddingAnimator;
    private readonly SimpleAnimator<Color4> colorAnimator;

    public ButtonEntity(FlexFrameworkMain engine) : base(engine)
    {
        textEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-regular"));
        textEntity.HorizontalAlignment = HorizontalAlignment.Left;

        colorAnimator = new SimpleAnimator<Color4>(
            (left, right, factor) =>
            {
                float t = (float) Easing.InOutQuad(factor);
                return new Color4(
                    MathHelper.Lerp(left.R, right.R, t),
                    MathHelper.Lerp(left.G, right.G, t),
                    MathHelper.Lerp(left.B, right.B, t),
                    MathHelper.Lerp(left.A, right.A, t));
            },
            value => textEntity.Color = value,
            () => TextUnfocusedColor,
            10.0);

        paddingAnimator = new SimpleAnimator<double>(
            (left, right, factor) => MathHelper.Lerp(left, right, Easing.InOutQuad(factor)),
            value => textPaddingX = value,
            () => 0.0,
            10.0);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        colorAnimator.Update(args.DeltaTime);
        paddingAnimator.Update(args.DeltaTime);

        if (IsFocused && Engine.Input.GetKeyDown(Keys.Enter))
        {
            PressedCallback?.Invoke();
        }
    }

    protected override void OnFocused()
    {
        colorAnimator.LerpTo(() => TextFocusedColor);
        paddingAnimator.LerpTo(() => 16.0);
    }

    protected override void OnUnfocused()
    {
        colorAnimator.LerpTo(() => TextUnfocusedColor);
        paddingAnimator.LerpTo(() => 0.0);
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0);

        matrixStack.Push();
        matrixStack.Translate(-Origin.X * Size.X, -Origin.Y * Size.Y, 0.0);
        matrixStack.Translate(TextPosOffset.X, TextPosOffset.Y, 0.0);
        matrixStack.Translate(textPaddingX, 0.0, 0.0);
        textEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
        
        matrixStack.Pop();
    }

    public override void Dispose()
    {
        textEntity.Dispose();
    }
}