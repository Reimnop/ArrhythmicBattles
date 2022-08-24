using FlexFramework.Core.Data;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.EntitySystem.Default;

public class MeshEntity : Entity, IRenderable
{
    public Mesh<Vertex>? Mesh { get; set; }
    public Color4 Color { get; set; } = Color4.White;

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        if (Mesh == null)
        {
            return;
        }
        
        Matrix4 transformation = (matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection).ToMatrix4();
        VertexDrawData vertexDrawData = new VertexDrawData(Mesh.VertexArray, Mesh.Count, transformation, Color);
        
        renderer.EnqueueDrawData(layerId, vertexDrawData);
    }
    
    public override void Dispose()
    {
    }
}