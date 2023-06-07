using FlexFramework.Core;
using FlexFramework.Core.Entities;
using Glide;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Intro;

public class BannerEntity : Entity, IRenderable
{
    private readonly LogoEntity logoEntity;
    private Vector2 logoScale = Vector2.One * 2.0f;
    private Vector2 logoOffset = Vector2.Zero;
    
    private readonly TitleEntity titleEntity = new();
    
    private float time;

    public float Time
    {
        get => time;
        set
        {
            time = value;

            var logoT = MathHelper.MapRange(value, 0.0f, 0.75f, 0.0f, 1.0f);
            var titleT = MathHelper.MapRange(value, 0.75f, 1.0f, 0.0f, 1.0f);
            var logoScaleT = MathHelper.MapRange(value, 0.25f, 0.75f, 0.0f, 1.0f);
            var logoOffsetT = MathHelper.MapRange(value, 0.25f, 0.75f, 0.0f, 1.0f);
            
            logoT = MathHelper.Clamp(logoT, 0.0f, 1.0f);
            titleT = MathHelper.Clamp(titleT, 0.0f, 1.0f);
            logoScaleT = MathHelper.Clamp(logoScaleT, 0.0f, 1.0f);
            logoOffsetT = MathHelper.Clamp(logoOffsetT, 0.0f, 1.0f);

            logoT = Ease.QuintInOut(logoT);
            titleT = Ease.QuintInOut(titleT);
            logoScaleT = Ease.QuintInOut(logoScaleT);
            logoOffsetT = Ease.QuintInOut(logoOffsetT);

            logoEntity.Time = logoT;
            titleEntity.Time = titleT;
            logoScale = Vector2.Lerp(Vector2.One * 2.0f, Vector2.One, logoScaleT);
            logoOffset = Vector2.Lerp(Vector2.Zero, new Vector2(-291.5f, 0.0f), logoOffsetT); // Magic numbers from Figma
        }
    }

    public BannerEntity()
    {
        logoEntity = new LogoEntity()
        {
            Color = new Color4(247, 203, 41, 255)
        };

        Time = 0.0f;
    }

    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Scale(logoScale.X, logoScale.Y, 1.0f); // Scale the logo
        matrixStack.Translate(logoOffset.X, logoOffset.Y, 0.0f); // Offset the logo
        logoEntity.Render(args);
        matrixStack.Pop();
        
        matrixStack.Push();
        matrixStack.Translate(92.0f, 0.0f, 0.0f); // More magic numbers from Figma
        titleEntity.Render(args);
        matrixStack.Pop();
    }
}