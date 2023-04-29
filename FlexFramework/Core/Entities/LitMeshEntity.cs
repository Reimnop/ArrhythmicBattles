using FlexFramework.Core.Data;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class LitMeshEntity : Entity, IRenderable
{
    public Mesh<LitVertex>? Mesh { get; set; }
    public Texture? Texture { get; set; }
    public Color4 Color { get; set; } = Color4.White;

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
        LitVertexDrawData vertexDrawData = new LitVertexDrawData(Mesh.ReadOnly, matrixStack.GlobalTransformation, transformation, Texture?.ReadOnly, Color);

        renderer.EnqueueDrawData(layerId, vertexDrawData);
    }
}