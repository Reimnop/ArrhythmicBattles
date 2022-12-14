using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

namespace ArrhythmicBattles.UI;

public class SliderEntity : UIElement, IRenderable
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
    
    public event Action<int>? OnValueChanged;

    public Color4 UnfocusedColor { get; set; } = Color4.White;
    public Color4 FocusedColor { get; set; } = Color4.Black;

    public Vector2i TextPosOffset { get; set; }
    public Vector2i BarPosOffset { get; set; }

    public int BarsCount { get; set; } = 10;
    public int Value { get; set; } = 10;

    private readonly InputSystem input;
    private readonly InputCapture capture;
    
    private readonly SimpleAnimator<Color4> colorAnimator;
    
    private readonly TextEntity textEntity;
    private readonly MeshEntity barMeshEntity;
    private readonly MeshEntity barMeshEntityLowOpacity;

    private InputCapture? modifyCapture;

    public SliderEntity(FlexFrameworkMain engine, InputInfo inputInfo) : base(engine)
    {
        input = inputInfo.InputSystem;
        capture = inputInfo.InputCapture;

        Font font = engine.TextResources.GetFont("inconsolata-regular");
        
        textEntity = new TextEntity(engine, font);
        barMeshEntity = new MeshEntity();
        barMeshEntity.Mesh = engine.PersistentResources.QuadMesh;
        barMeshEntityLowOpacity = new MeshEntity();
        barMeshEntityLowOpacity.Mesh = engine.PersistentResources.QuadMesh;

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
            value =>
            {
                textEntity.Color = value;
                barMeshEntity.Color = value;
                barMeshEntityLowOpacity.Color = new Color4(value.R, value.G, value.B, value.A * 0.25f);
            },
            () => UnfocusedColor,
            10.0f);
    }

    public void InitMaxValue(int value)
    {
        BarsCount = value;
        Value = value;
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        colorAnimator.Update(args.DeltaTime);

        if (IsFocused && input.GetKeyDown(capture, Keys.Enter))
        {
            modifyCapture = input.AcquireCapture();
        } 
        else if (modifyCapture != null)
        {
            if (input.GetKeyDown(modifyCapture, Keys.Left))
            {
                Value--;
                Value = Math.Clamp(Value, 0, BarsCount);
                OnValueChanged?.Invoke(Value);
            }
            
            if (input.GetKeyDown(modifyCapture, Keys.Right))
            {
                Value++;
                Value = Math.Clamp(Value, 0, BarsCount);
                OnValueChanged?.Invoke(Value);
            }
            
            if (input.GetKeyDown(modifyCapture, Keys.Enter))
            {
                modifyCapture.Dispose();
                modifyCapture = null;
            }
        }
    }

    protected override void OnFocused()
    {
        colorAnimator.LerpTo(() => FocusedColor);
    }

    protected override void OnUnfocused()
    {
        colorAnimator.LerpTo(() => UnfocusedColor);
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0f);

        // draw title
        matrixStack.Push();
        matrixStack.Translate(TextPosOffset.X, TextPosOffset.Y, 0.0f);
        matrixStack.Push();
        matrixStack.Translate(-Origin.X * Size.X, -Origin.Y * Size.Y, 0.0f);
        textEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
        matrixStack.Pop();

        // draw bars
        matrixStack.Push();
        matrixStack.Translate(BarPosOffset.X, BarPosOffset.Y, 0.0f);
        for (int i = 0; i < Value; i++)
        {
            matrixStack.Push();
            matrixStack.Translate(0.5f, 0.5f, 0.0f);
            matrixStack.Scale(16.0f, 24.0f, 0.0f);
            matrixStack.Translate(i * 20.0f, 0.0f, 0.0f);
            barMeshEntity.Render(renderer, layerId, matrixStack, cameraData);
            matrixStack.Pop();
        }
        matrixStack.Pop();
        
        // draw bars with lower opacity
        matrixStack.Push();
        matrixStack.Translate(BarPosOffset.X, BarPosOffset.Y, 0.0f);
        for (int i = Value; i < BarsCount; i++)
        {
            matrixStack.Push();
            matrixStack.Translate(0.5f, 0.5f, 0.0f);
            matrixStack.Scale(16.0f, 24.0f, 0.0f);
            matrixStack.Translate(i * 20.0f, 0.0f, 0.0f);
            barMeshEntityLowOpacity.Render(renderer, layerId, matrixStack, cameraData);
            matrixStack.Pop();
        }
        matrixStack.Pop();

        matrixStack.Pop();
    }

    public override void Dispose()
    {
        modifyCapture?.Dispose();
        textEntity.Dispose();
    }
}