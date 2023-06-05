using System.Diagnostics;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Intro;

// A small price to pay for salvation
// This is the worst way to render an animated logo, but it's simple and it works
public class LogoEntity : Entity, IRenderable
{
    private static readonly Box2 Rect1Initial = new(120.0f, 0.0f, 120.0f, 26.0f);
    private static readonly Box2 Rect2Initial = new(73.0f, 48.0f, 73.0f, 74.0f);
    private static readonly Box2 Rect3Initial = new(0.0f, 74.0f, 26.0f, 74.0f);
    private static readonly Box2 Rect4Initial = new(26.0f, 95.0f, 26.0f, 120.0f);
    private static readonly Box2 Rect5Initial = new(94.0f, 95.0f, 120.0f, 95.0f);
    
    // Values copied from Figma (https://www.figma.com/file/uyOZdXFvE3em6bkHRxyiHe/Arrhythmic-Battles)
    private static readonly Box2 Rect1Final = new(47.0f, 0.0f, 120.0f, 26.0f);
    private static readonly Box2 Rect2Final = new(0.0f, 48.0f, 73.0f, 74.0f);
    private static readonly Box2 Rect3Final = new(0.0f, 74.0f, 26.0f, 120.0f);
    private static readonly Box2 Rect4Final = new(26.0f, 95.0f, 120.0f, 120.0f);
    private static readonly Box2 Rect5Final = new(94.0f, 48.0f, 120.0f, 95.0f);

    private readonly RectEntity rect1;
    private readonly RectEntity rect2;
    private readonly RectEntity rect3;
    private readonly RectEntity rect4;
    private readonly RectEntity rect5;
    
    public Color4 Color
    {
        get
        {
            // Assert that all colors are the same
            Debug.Assert(rect1.Color == rect2.Color);
            Debug.Assert(rect1.Color == rect3.Color);
            Debug.Assert(rect1.Color == rect4.Color);
            Debug.Assert(rect1.Color == rect5.Color);

            return rect1.Color;
        }
        set
        {
            rect1.Color = value;
            rect2.Color = value;
            rect3.Color = value;
            rect4.Color = value;
            rect5.Color = value;
        }
    }

    public LogoEntity()
    {
        rect1 = new RectEntity()
        {
            Bounds = Rect1Initial
        };

        rect2 = new RectEntity()
        {
            Bounds = Rect2Initial
        };

        rect3 = new RectEntity()
        {
            Bounds = Rect3Initial
        };

        rect4 = new RectEntity()
        {
            Bounds = Rect4Initial
        };

        rect5 = new RectEntity()
        {
            Bounds = Rect5Initial
        };
    }

    public void SetAnimationTime(float t)
    {
        // rect1 t: 0 -> 1
        // rect2 t: 0 -> 73/260
        // rect3 t: 73/260 -> 119/260
        // rect4 t: 119/260 -> 213/260
        // rect5 t: 213/260 -> 1

        var rect1T = t;
        var rect2T = MathHelper.MapRange(t, 0.0f, 73.0f / 260.0f, 0.0f, 1.0f);
        var rect3T = MathHelper.MapRange(t, 73.0f / 260.0f, 119.0f / 260.0f, 0.0f, 1.0f);
        var rect4T = MathHelper.MapRange(t, 119.0f / 260.0f, 213.0f / 260.0f, 0.0f, 1.0f);
        var rect5T = MathHelper.MapRange(t, 213.0f / 260.0f, 1.0f, 0.0f, 1.0f);
        
        rect1.Bounds = LerpBox2(Rect1Initial, Rect1Final, rect1T);
        rect2.Bounds = LerpBox2(Rect2Initial, Rect2Final, rect2T);
        rect3.Bounds = LerpBox2(Rect3Initial, Rect3Final, rect3T);
        rect4.Bounds = LerpBox2(Rect4Initial, Rect4Final, rect4T);
        rect5.Bounds = LerpBox2(Rect5Initial, Rect5Final, rect5T);
    }

    private static Box2 LerpBox2(Box2 a, Box2 b, float t)
    {
        return new Box2(
            MathHelper.Lerp(a.Min.X, b.Min.X, t),
            MathHelper.Lerp(a.Min.Y, b.Min.Y, t),
            MathHelper.Lerp(a.Max.X, b.Max.X, t),
            MathHelper.Lerp(a.Max.Y, b.Max.Y, t));
    }

    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(-60.0f, -60.0f, 0.0f);
        matrixStack.Rotate(Vector3.UnitZ, MathF.PI / 4.0f); // 45 degrees
        
        rect1.Render(args);
        rect2.Render(args);
        rect3.Render(args);
        rect4.Render(args);
        rect5.Render(args);
        
        matrixStack.Pop();
    }
}