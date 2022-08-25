using FlexFramework;
using FlexFramework.Core.Data;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.EntitySystem.Default;

public enum ImageMode
{
    Fill,
    Fit,
    Stretch
}

public class ImageEntity : UIElement, IRenderable
{
    public override Vector2d Position { get; set; } = Vector2d.Zero;
    public override Vector2d Size { get; set; } = Vector2d.One * 128.0;
    public override Vector2d Origin { get; set; } = Vector2d.Zero;
    public override bool IsFocused { get; set; }

    public Texture2D? Texture { get; set; }
    public ImageMode ImageMode { get; set; } = ImageMode.Fill;
    public Color4 Color { get; set; } = Color4.White;

    private readonly Mesh<Vertex> quadMesh;

    public ImageEntity(FlexFrameworkMain engine) : base(engine)
    {
        quadMesh = engine.PersistentResources.QuadMesh;
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        if (Texture == null)
        {
            return;
        }

        matrixStack.Push();
        matrixStack.Translate(0.5 - Origin.X, 0.5 - Origin.Y, 0.0);
        switch (ImageMode)
        {
            case ImageMode.Fill:
                matrixStack.Scale(Size.X, Size.Y, 1.0);
                break;
            case ImageMode.Fit:
                if (Size.X / (double) Size.Y > Texture.Width / (double) Texture.Height)
                {
                    matrixStack.Scale(Size.Y * Texture.Width / (double) Texture.Height, Size.Y, 1.0);
                }
                else
                {
                    matrixStack.Scale(Size.X, Size.X * Texture.Height / (double) Texture.Width, 1.0);
                }
                break;
            case ImageMode.Stretch:
                if (Size.X / (double) Size.Y > Texture.Width / (double) Texture.Height)
                {
                    matrixStack.Scale(Size.X, Size.X * Texture.Height / (double) Texture.Width, 1.0);
                }
                else
                {
                    matrixStack.Scale(Size.Y * Texture.Width / (double) Texture.Height, Size.Y, 1.0);
                }
                break;
        }
        matrixStack.Translate(Position.X, Position.Y, 0.0);
        
        Matrix4 transformation = (matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection).ToMatrix4();
        VertexDrawData vertexDrawData = new VertexDrawData(quadMesh.VertexArray, quadMesh.Count, transformation, Texture, Color);

        renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
    }

    public override void Dispose()
    {
    }
}