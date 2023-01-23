using FlexFramework;
using FlexFramework.Core.Data;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
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
    public override Vector2 Position { get; set; } = Vector2.Zero;
    public override Vector2 Size { get; set; } = Vector2.One * 128.0f;
    public override Vector2 Origin { get; set; } = Vector2.Zero;
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
        matrixStack.Translate(0.5f - Origin.X, 0.5f - Origin.Y, 0.0f);
        switch (ImageMode)
        {
            case ImageMode.Fill:
                matrixStack.Scale(Size.X, Size.Y, 1.0f);
                break;
            case ImageMode.Fit:
                if (Size.X / Size.Y > Texture.Width / (float) Texture.Height)
                {
                    matrixStack.Scale(Size.Y * Texture.Width / Texture.Height, Size.Y, 1.0f);
                }
                else
                {
                    matrixStack.Scale(Size.X, Size.X * Texture.Height / Texture.Width, 1.0f);
                }
                break;
            case ImageMode.Stretch:
                if (Size.X / Size.Y > Texture.Width / (float) Texture.Height)
                {
                    matrixStack.Scale(Size.X, Size.X * Texture.Height / Texture.Width, 1.0f);
                }
                else
                {
                    matrixStack.Scale(Size.Y * Texture.Width / Texture.Height, Size.Y, 1.0f);
                }
                break;
        }
        matrixStack.Translate(Position.X, Position.Y, 0.0f);
        
        Matrix4 transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        VertexDrawData vertexDrawData = new VertexDrawData(quadMesh.VertexArray, quadMesh.Count, transformation, Texture, Color);

        renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
    }
}