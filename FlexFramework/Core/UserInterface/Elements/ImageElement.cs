using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class ImageElement : Element, IRenderable
{
    public ImageMode ImageMode
    {
        get => imageEntity.ImageMode;
        set => imageEntity.ImageMode = value;
    }
    
    private Vector2 min;
    private readonly ImageEntity imageEntity;

    public ImageElement(Texture texture)
    {
        imageEntity = new ImageEntity(texture);
    }
    
    protected override void UpdateLayout(Box2 bounds)
    {
        min = bounds.Min;
        imageEntity.Size = bounds.Size;
    }

    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(min.X, min.Y, 0.0f);
        imageEntity.Render(args);
        matrixStack.Pop();
    }
}