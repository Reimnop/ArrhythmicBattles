using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class MeshEntity : Entity, IRenderable
{
    public Mesh<Vertex> Mesh { get; set; }
    public Texture? Texture { get; set; }
    public Color4 Color { get; set; } = Color4.White;
    public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Triangles;

    public MeshEntity(Mesh<Vertex> mesh)
    {
        Mesh = mesh;
    }

    public void Render(RenderArgs args)
    {
        var commandList = args.CommandList;
        var layerType = args.LayerType;
        var matrixStack = args.MatrixStack;
        var cameraData = args.CameraData;

        var transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        var vertexDrawData = new VertexDrawData(Mesh.ReadOnly, transformation, Texture?.ReadOnly, Color, PrimitiveType);
        
        commandList.AddDrawData(layerType, vertexDrawData);
    }
}