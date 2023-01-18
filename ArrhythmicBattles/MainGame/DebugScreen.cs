﻿using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;
using Textwriter;

namespace ArrhythmicBattles.MainGame;

public class DebugScreen : Screen
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
    
    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0f);
        leftTextEntity.Render(renderer, layerId, matrixStack, cameraData);
        
        matrixStack.Push();
        matrixStack.Translate(engine.ClientSize.X, 0.0f, 0.0f);
        rightTextEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
        
        matrixStack.Pop();
    }

    public override void Dispose()
    {
        leftTextEntity.Dispose();
    }
}