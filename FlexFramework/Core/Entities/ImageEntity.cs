using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class ImageEntity : Entity, IRenderable
{
    public Texture Texture { get; set; }
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
        var transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        var vertexDrawData = new VertexDrawData(DefaultAssets.QuadMesh.ReadOnly, transformation, Texture?.ReadOnly, Color, PrimitiveType.Triangles);
        commandList.AddDrawData(layerType, vertexDrawData);
    }
}