using FlexFramework;
using FlexFramework.Core.Data;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UI;

public enum ImageMode
{
    Fill,
    Fit,
    Stretch
}

public class ImageEntity : UIElement, IRenderable
{
    public override Vector2i Position { get; set; } = Vector2i.Zero;
    public override Vector2i Size { get; set; } = Vector2i.One * 128;
    public override Vector2d Origin { get; set; } = Vector2d.Zero;
    public override bool IsFocused { get; set; }

    public Texture2D Texture { get; set; }
    public ImageMode ImageMode { get; set; } = ImageMode.Fill;
    public Color4 Color { get; set; } = Color4.White;

    private readonly Mesh<Vertex> quadMesh;
    private readonly TexturedVertexDrawData vertexDrawData;

    public ImageEntity(FlexFrameworkMain engine) : base(engine)
    {
        quadMesh = engine.PersistentResources.QuadMesh;
        vertexDrawData = new TexturedVertexDrawData(quadMesh.VertexArray, quadMesh.Count, Matrix4.Identity, null, Color);
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        if (Texture == null)
        {
            return;
        }

        vertexDrawData.Color = Color;
        
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

        vertexDrawData.Transformation = (matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection).ToMatrix4();
        vertexDrawData.Texture = Texture;
        vertexDrawData.Color = Color;
        
        renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
    }

    public override void Dispose()
    {
    }
}