using FlexFramework.Core.Data;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class MeshEntity : Entity, IRenderable
{
    public Mesh<Vertex>? Mesh { get; set; }
    public Texture2D? Texture { get; set; }
    public Color4 Color { get; set; } = Color4.White;
    public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Triangles;

    public void Render(RenderArgs args)
    {
        if (Mesh == null)
        {
            return;
        }
        
        Renderer renderer = args.Renderer;
        int layerId = args.LayerId;
        MatrixStack matrixStack = args.MatrixStack;
        CameraData cameraData = args.CameraData;

        Matrix4 transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        VertexDrawData vertexDrawData = new VertexDrawData(Mesh, transformation, Texture, Color, PrimitiveType);
        
        renderer.EnqueueDrawData(layerId, vertexDrawData);
    }
}