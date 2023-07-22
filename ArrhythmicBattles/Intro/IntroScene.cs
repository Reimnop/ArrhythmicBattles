using ArrhythmicBattles.Core;
using ArrhythmicBattles.Menu;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using HalfMaid.Async;

namespace ArrhythmicBattles.Intro;

public class IntroScene : ABScene
{
    private readonly BannerEntity bannerEntity;
    
    private readonly GuiCamera guiCamera = new();
    private readonly MatrixStack matrixStack = new();

    public IntroScene(ABContext context) : base(context)
    {
        bannerEntity = new BannerEntity(context.ResourceManager);
        Context.TaskManager.StartImmediately(Animate);
    }

    private async GameTask Animate()
    {
        var taskManager = Context.TaskManager;
        
        await taskManager.WaitSeconds(0.25f);
        await taskManager.RunForSecondsNormalized(3.5f, t => bannerEntity.Time = t);
        await taskManager.WaitSeconds(0.25f);
        await taskManager.RunForSecondsNormalized(3.5f, t => bannerEntity.Time = 1.0f - t);
        await taskManager.WaitSeconds(0.25f);
        Engine.SceneManager.LoadScene(() => new MainMenuScene(Context));
    }

    protected override void RenderScene(CommandList commandList)
    {
        var cameraData = guiCamera.GetCameraData(Engine.ClientSize);
        var args = new RenderArgs(commandList, LayerType.Gui, matrixStack, cameraData);

        matrixStack.Push();
        matrixStack.Translate(Engine.ClientSize.X >> 1, Engine.ClientSize.Y >> 1, 0.0f); // Center the banner
        bannerEntity.Render(args);
        matrixStack.Pop();
    }

    public override void Update(UpdateArgs args)
    {
        // Do nothing
    }
}