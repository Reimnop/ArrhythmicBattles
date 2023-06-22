using ArrhythmicBattles.Core;
using FlexFramework;
using FlexFramework.Core.Entities;
using FlexFramework.Core;
using FlexFramework.Text;

namespace ArrhythmicBattles.Game;

public class DebugScreen : IUpdateable, IRenderable
{
    private readonly TextEntity leftTextEntity;
    private readonly TextEntity rightTextEntity;
    
    private readonly FlexFrameworkMain engine;

    private float time = 0.0f;
    
    private int fps = 0;
    private int counter = 0;

    public DebugScreen(FlexFrameworkMain engine, ABScene scene)
    {
        this.engine = engine;
        var gpuInfo = engine.Renderer.GpuInfo;
        
        var resourceManager = scene.Context.ResourceManager;
        var font = resourceManager.Get<Font>(Constants.RegularFontPath);
        
        leftTextEntity = new TextEntity(font);

        rightTextEntity = new TextEntity(font)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            Text = $"[INFO]\n\n" +
                   $"GPU: {gpuInfo.Name}\n" +
                   $"Vendor: {gpuInfo.Vendor}\n" +
                   $"Version: {gpuInfo.Version}\n"
        };
    }

    public void Update(UpdateArgs args)
    {
        time += args.DeltaTime;
        
        if (time >= 1.0f)
        {
            fps = counter;
            counter = 0;
            time = 0.0f;
        }
        else
        {
            counter++;
        }
        
        leftTextEntity.Text = $"[DEBUG]\n\n" +
                              $"Delta time: {args.DeltaTime * 1000.0f:0.0}ms\n" +
                              $"FPS: {fps}\n\n" +
                              $"\"uwaaa <3\"\n" +
                              $"    - Windows 98, the vg moderator.";
    }
    
    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        leftTextEntity.Render(args);
        
        matrixStack.Push();
        matrixStack.Translate(engine.ClientSize.X, 0.0f, 0.0f);
        rightTextEntity.Render(args);
        matrixStack.Pop();
        
        matrixStack.Pop();
    }
}