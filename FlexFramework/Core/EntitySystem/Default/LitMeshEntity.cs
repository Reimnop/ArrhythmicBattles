using FlexFramework.Core.Data;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.EntitySystem.Default;

public class LitMeshEntity : Entity, IRenderable
{
    public IndexedMesh<LitVertex>? Mesh { get; set; }
    public Texture2D? Texture { get; set; }
    public Color4 Color { get; set; } = Color4.White;

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        if (Mesh == null)
        {
            return;
        }

        Matrix4 transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        LitVertexDrawData vertexDrawData = new LitVertexDrawData(Mesh.VertexArray, Mesh.Count, matrixStack.GlobalTransformation, transformation, Texture, Color);

        renderer.EnqueueDrawData(layerId, vertexDrawData);
    }

    public override void Dispose()
    {
    }
}