﻿using ArrhythmicBattles.Util;
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

    private readonly InputSystem input;
    private readonly InputCapture capture;
    private readonly TextEntity textEntity;
    private readonly SimpleAnimator<Color4> colorAnimator;

    public ButtonEntity(FlexFrameworkMain engine, InputInfo inputInfo) : base(engine)
    {
        input = inputInfo.InputSystem;
        capture = inputInfo.InputCapture;
        
        textEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-regular"));
        textEntity.HorizontalAlignment = HorizontalAlignment.Left;

        colorAnimator = new SimpleAnimator<Color4>(
            (left, right, factor) =>
            {
                float t = (float) Easing.QuadInOut(factor);
                return new Color4(
                    MathHelper.Lerp(left.R, right.R, t),
                    MathHelper.Lerp(left.G, right.G, t),
                    MathHelper.Lerp(left.B, right.B, t),
                    MathHelper.Lerp(left.A, right.A, t));
            },
            value => textEntity.Color = value,
            () => TextUnfocusedColor,
            10.0);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        colorAnimator.Update(args.DeltaTime);

        if (IsFocused && input.GetKeyDown(capture, Keys.Enter))
        {
            PressedCallback?.Invoke();
        }
    }

    protected override void OnFocused()
    {
        colorAnimator.LerpTo(() => TextFocusedColor);
    }

    protected override void OnUnfocused()
    {
        colorAnimator.LerpTo(() => TextUnfocusedColor);
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0);

        matrixStack.Push();
        matrixStack.Translate(-Origin.X * Size.X, -Origin.Y * Size.Y, 0.0);
        matrixStack.Translate(TextPosOffset.X, TextPosOffset.Y, 0.0);
        textEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
        
        matrixStack.Pop();
    }

    public override void Dispose()
    {
        textEntity.Dispose();
    }
}