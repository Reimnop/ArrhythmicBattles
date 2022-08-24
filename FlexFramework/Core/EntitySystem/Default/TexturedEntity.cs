using FlexFramework.Core.Data;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using FlexFramework.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.EntitySystem.Default;

public class TexturedEntity : Entity, IRenderable
{
    public Texture2D? Texture { get; set; }
    public bool MaintainAspectRatio { get; set; } = true;
    public Color4 Color { get; set; } = Color4.White;

    private readonly Mesh<Vertex> quadMesh;

    public TexturedEntity(FlexFrameworkMain engine)
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
        if (MaintainAspectRatio)
        {
            matrixStack.Scale(Texture.Width / (double) Texture.Height, 1.0, 1.0);
        }
        
        Matrix4 transformation = (matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection).ToMatrix4();
        TexturedVertexDrawData vertexDrawData = new TexturedVertexDrawData(quadMesh.VertexArray, quadMesh.Count, transformation, Texture, Color);

        renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
    }
    
    public override void Dispose()
    {
    }
}