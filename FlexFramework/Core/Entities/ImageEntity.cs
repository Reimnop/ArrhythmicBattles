using FlexFramework;
using FlexFramework.Core.Data;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public enum ImageMode
{
    Fill,
    Fit,
    Stretch
}

public class ImageEntity : Entity, IRenderable
{
    public Vector2 Size { get; set; } = Vector2.One * 128.0f;

    public Texture Texture { get; set; }
    public ImageMode ImageMode { get; set; } = ImageMode.Fill;
    public Color4 Color { get; set; } = Color4.White;

    public ImageEntity(Texture texture)
    {
        Texture = texture;
    }

    public void Render(RenderArgs args)
    {
        var commandList = args.CommandList;
        var layerType = args.LayerType;
        var matrixStack = args.MatrixStack;
        var cameraData = args.CameraData;

        matrixStack.Push();
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

        var transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        var vertexDrawData = new VertexDrawData(DefaultAssets.QuadMesh.ReadOnly, transformation, Texture?.ReadOnly, Color, PrimitiveType.Triangles);

        commandList.AddDrawData(layerType, vertexDrawData);
        matrixStack.Pop();
    }
}