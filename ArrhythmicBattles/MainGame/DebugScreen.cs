using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.MainGame;

public class DebugScreen : Screen
{
    public override Vector2 Position { get; set; }
    
    private readonly TextEntity textEntity;
    private readonly ABScene scene;
    
    private float time = 0.0f;
    
    private int fps = 0;
    private int counter = 0;

    public DebugScreen(FlexFrameworkMain engine, ABScene scene)
    {
        this.scene = scene;

        textEntity = new TextEntity(engine, engine.TextResources.GetFont("inconsolata-small"));
        textEntity.BaselineOffset = 18.0f;
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
        
        textEntity.Text = $"[DEBUG]\n\nDelta time: {args.DeltaTime * 1000.0f:0.0}ms\nFPS: {fps}";
        
        textEntity.Update(args);
    }
    
    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(Position.X, Position.Y, 0.0f);
        textEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }

    public override void Dispose()
    {
        textEntity.Dispose();
    }
}