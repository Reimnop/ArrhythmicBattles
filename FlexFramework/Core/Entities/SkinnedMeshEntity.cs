using FlexFramework.Core.Data;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class SkinnedMeshEntity : Entity, IRenderable
{
    public IndexedMesh<SkinnedVertex>? Mesh { get; set; }
    public Texture2D? Texture { get; set; }
    public Matrix4[]? Bones { get; set; }
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
        SkinnedVertexDrawData vertexDrawData = new SkinnedVertexDrawData(Mesh.VertexArray, Mesh.Count, matrixStack.GlobalTransformation, transformation, Texture, Bones, Color);
        
        renderer.EnqueueDrawData(layerId, vertexDrawData);
    }
}