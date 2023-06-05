using ArrhythmicBattles.Core;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using Glide;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Intro;

public class IntroScene : ABScene
{
    private readonly LogoEntity logoEntity;
    private float time;

    private Vector2 logoScale;

    public IntroScene(ABContext context) : base(context)
    {
        logoEntity = new LogoEntity()
        {
            Color = new Color4(247, 203, 41, 255)
        };
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        time += args.DeltaTime;
        
        var animTime = MathHelper.Clamp(time / 3.0f - 1.0f, 0.0f, 1.0f);
        var t = Ease.QuintInOut(animTime);
        logoEntity.SetAnimationTime(t);
        logoScale = Vector2.Lerp(Vector2.One * 2.5f, Vector2.One * 2.0f, t);
    }

    protected override void RenderScene(CommandList commandList)
    {
        var cameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        var args = new RenderArgs(commandList, LayerType.Gui, MatrixStack, cameraData);

        MatrixStack.Push();
        MatrixStack.Scale(logoScale.X, logoScale.Y, 0.0f);
        MatrixStack.Translate(Engine.ClientSize.X / 2.0f, Engine.ClientSize.Y / 2.0f - 60.0f, 0.0f); // Center the logo, with a slight offset
        
        // Render the logo
        logoEntity.Render(args);
        MatrixStack.Pop();
    }
}