using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering;
using FlexFramework.Core;
using OpenTK.Mathematics;
using Textwriter;

namespace ArrhythmicBattles.Game;

public class DebugScreen : Screen, IDisposable
{
    public override Vector2 Position { get; set; }

    private readonly TextEntity leftTextEntity;
    private readonly TextEntity rightTextEntity;
    
    private readonly FlexFrameworkMain engine;
    private readonly ABScene scene;
    
    private float time = 0.0f;
    
    private int fps = 0;
    private int counter = 0;

    public DebugScreen(FlexFrameworkMain engine, ABScene scene)
    {
        this.engine = engine;
        this.scene = scene;

        GpuInfo gpuInfo = engine.Renderer.GpuInfo;

        leftTextEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-small"));
        leftTextEntity.BaselineOffset = 16.0f;
        
        rightTextEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-small"));
        rightTextEntity.BaselineOffset = 16.0f;
        rightTextEntity.HorizontalAlignment = HorizontalAlignment.Right;
        rightTextEntity.Text = $"[INFO]\n\n" +
                               $"GPU: {gpuInfo.Name}\n" +
                               $"Vendor: {gpuInfo.Vendor}\n" +
                               $"Version: {gpuInfo.Version}\n";
    }
    
    public override void Update(UpdateArgs args)
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
        
        leftTextEntity.Update(args);
        rightTextEntity.Update(args);
    }
    
    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0f);
        leftTextEntity.Render(args);
        
        matrixStack.Push();
        matrixStack.Translate(engine.ClientSize.X, 0.0f, 0.0f);
        rightTextEntity.Render(args);
        matrixStack.Pop();
        
        matrixStack.Pop();
    }

    public void Dispose()
    {
        leftTextEntity.Dispose();
    }
}