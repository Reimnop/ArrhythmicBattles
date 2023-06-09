using ArrhythmicBattles.Core;
using ArrhythmicBattles.Menu;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using Glide;

namespace ArrhythmicBattles.Intro;

public class IntroScene : ABScene
{
    private readonly BannerEntity bannerEntity;
    private readonly Tweener tweener = new();

    public IntroScene(ABContext context) : base(context)
    {
        bannerEntity = new BannerEntity(context.ResourceManager);
        
        tweener
            .Tween(bannerEntity, new {Time = 1.0f}, 3.5f, 0.25f)
            .Repeat(1)
            .Reflect()
            .OnComplete(() => Engine.LoadScene(new MainMenuScene(context)));
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);

        tweener.Update(args.DeltaTime);
    }

    protected override void RenderScene(CommandList commandList)
    {
        var cameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        var args = new RenderArgs(commandList, LayerType.Gui, MatrixStack, cameraData);

        MatrixStack.Push();
        MatrixStack.Translate(Engine.ClientSize.X >> 1, Engine.ClientSize.Y >> 1, 0.0f); // Center the banner
        bannerEntity.Render(args);
        MatrixStack.Pop();
    }
}