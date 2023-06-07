using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Intro;

public class TitleEntity : Entity, IRenderable
{
    private static readonly Vector2 TitleSize = new(569.0f, 137.0f);
    private static readonly Box2 RevealRect1 = new(-TitleSize.X / 2.0f, -TitleSize.Y / 2.0f, -TitleSize.X / 2.0f, TitleSize.Y / 2.0f);
    private static readonly Box2 RevealRect2 = new(-TitleSize.X / 2.0f, -TitleSize.Y / 2.0f, TitleSize.X / 2.0f, TitleSize.Y / 2.0f);
    private static readonly Box2 RevealRect3 = new(TitleSize.X / 2.0f, -TitleSize.Y / 2.0f, TitleSize.X / 2.0f, TitleSize.Y / 2.0f);
    
    private RectEntity revealRect;
    private ImageEntity titleImage;

    private float time;

    public float Time
    {
        get => time;
        set
        {
            time = value;

            var boxFrom = value < 0.5f ? RevealRect1 : RevealRect2;
            var boxTo = value < 0.5f ? RevealRect2 : RevealRect3;
            var t = value < 0.5f ? value * 2.0f : MathHelper.Clamp(value * 2.0f - 1.0f, 0.0f, 1.0f);
            revealRect.Bounds = LerpBox2(boxFrom, boxTo, t);
        }
    }
    
    public TitleEntity()
    {
        revealRect = new RectEntity()
        {
            Color = Color4.White,
            Bounds = RevealRect1
        };

        var texture = TextureSampler
            .FromFile("title", "Assets/Title.png")
            .SetWrapMode(WrapMode.ClampToBorder)
            .SetBorderColor(Color4.Transparent);
        titleImage = new ImageEntity(texture);
    }
    
    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;

        if (Time > 0.5f)
        {
            matrixStack.Push();
            matrixStack.Scale(TitleSize.X, TitleSize.Y, 0.0f);
            titleImage.Render(args);
            matrixStack.Pop();
        }

        revealRect.Render(args);
    }
    
    private static Box2 LerpBox2(Box2 a, Box2 b, float t)
    {
        return new Box2(
            MathHelper.Lerp(a.Min.X, b.Min.X, t),
            MathHelper.Lerp(a.Min.Y, b.Min.Y, t),
            MathHelper.Lerp(a.Max.X, b.Max.X, t),
            MathHelper.Lerp(a.Max.Y, b.Max.Y, t));
    }
}