using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Textwriter;

namespace ArrhythmicBattles.UI;

public class SliderEntity : UIElement, IRenderable, IDisposable
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

    private readonly IInputProvider inputProvider;

    private SimpleAnimator<Color4> colorAnimator = null!;
    
    private readonly TextEntity textEntity;
    private readonly MeshEntity barMeshEntity;
    private readonly MeshEntity barMeshEntityLowOpacity;
    
    private bool focused = false;

    public SliderEntity(FlexFrameworkMain engine, IInputProvider inputProvider) : base(engine)
    {
        this.inputProvider = inputProvider;

        Font font = engine.TextResources.GetFont("inconsolata-regular");
        
        EngineResources resources = engine.Resources;

        textEntity = new TextEntity(engine, font);
        barMeshEntity = new MeshEntity();
        barMeshEntity.Mesh = engine.ResourceManager.GetResource<Mesh<Vertex>>(resources.QuadMesh);;
        barMeshEntityLowOpacity = new MeshEntity();
        barMeshEntityLowOpacity.Mesh = engine.ResourceManager.GetResource<Mesh<Vertex>>(resources.QuadMesh);;
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
            value =>
            {
                textEntity.Color = value;
                barMeshEntity.Color = value;
                barMeshEntityLowOpacity.Color = new Color4(value.R, value.G, value.B, value.A * 0.25f);
            },
            UnfocusedColor,
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
        
        colorAnimator.Update(args);

        if (focused)
        {
            if (inputProvider.GetKeyDown(Keys.Left))
            {
                Value--;
                Value = Math.Clamp(Value, 0, BarsCount);
                OnValueChanged?.Invoke(Value);
            }
            
            if (inputProvider.GetKeyDown(Keys.Right))
            {
                Value++;
                Value = Math.Clamp(Value, 0, BarsCount);
                OnValueChanged?.Invoke(Value);
            }
        }
    }

    protected override void OnFocused()
    {
        colorAnimator.LerpTo(FocusedColor);
        focused = true;
    }

    protected override void OnUnfocused()
    {
        colorAnimator.LerpTo(UnfocusedColor);
        focused = false;
    }

    public void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0f);

        // draw title
        matrixStack.Push();
        matrixStack.Translate(TextPosOffset.X, TextPosOffset.Y, 0.0f);
        matrixStack.Push();
        matrixStack.Translate(-Origin.X * Size.X, -Origin.Y * Size.Y, 0.0f);
        textEntity.Render(args);
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
            barMeshEntity.Render(args);
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
            barMeshEntityLowOpacity.Render(args);
            matrixStack.Pop();
        }
        matrixStack.Pop();

        matrixStack.Pop();
    }

    public void Dispose()
    {
        textEntity.Dispose();
    }
}