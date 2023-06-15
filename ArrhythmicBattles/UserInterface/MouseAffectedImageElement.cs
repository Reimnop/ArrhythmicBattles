using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UserInterface;

public class MouseAffectedImageElement : Element, IUpdateable, IRenderable
{
    private readonly ImageEntity imageEntity;
    private readonly IInputProvider inputProvider;
    private readonly float imageAspect;

    private Box2 bounds;
    private Vector2 mousePosition;

    public MouseAffectedImageElement(IInputProvider inputProvider, TextureSampler texture)
    {
        imageEntity = new ImageEntity(texture);
        this.inputProvider = inputProvider;
        imageAspect = texture.Width / (float) texture.Height;
    }
    
    protected override void UpdateLayout(Box2 bounds)
    {
        this.bounds = bounds;
    }
    
    public void Update(UpdateArgs args)
    {
        var pos = inputProvider.MousePosition;
        
        // Clamp position to bounds
        pos = Vector2.Clamp(pos, bounds.Min, bounds.Max);
        
        // Smooth mouse movement
        mousePosition = Vector2.Lerp(mousePosition, pos, args.DeltaTime * 6.0f);
    }

    public void Render(RenderArgs args)
    {
        var boundsCenter = bounds.Center;
        var boundsSize = bounds.Size;
        var size = boundsSize.X / boundsSize.Y > imageAspect ? new Vector2(boundsSize.X, boundsSize.X / imageAspect) : new Vector2(boundsSize.Y * imageAspect, boundsSize.Y);
        size *= 1.05f;
        
        var mouseMoveMax = MathF.Min(boundsSize.X, boundsSize.Y) * 0.05f;
        var mousePosRelative = mousePosition - boundsCenter;
        var position = mousePosRelative / boundsSize * mouseMoveMax + boundsCenter;

        var matrixStack = args.MatrixStack;
        matrixStack.Push();
        matrixStack.Scale(size.X, size.Y, 1.0f);
        matrixStack.Translate(position.X, position.Y, 0.0f);
        imageEntity.Render(args);
        matrixStack.Pop();
    }
}