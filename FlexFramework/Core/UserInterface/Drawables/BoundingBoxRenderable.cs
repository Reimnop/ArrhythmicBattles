using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Drawables;

public class BoundingBoxRenderable : IRenderable
{
    private readonly Mesh<Vertex> mesh;
    private readonly Bounds bounds;
    
    public BoundingBoxRenderable(FlexFrameworkMain engine, Bounds bounds)
    {
        this.bounds = bounds;
        
        EngineResources resources = engine.Resources;
        mesh = engine.ResourceManager.GetResource<Mesh<Vertex>>(resources.QuadWireframeMesh);
    }
    
    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(bounds.X0, bounds.Y0, 0.0f);
        matrixStack.Scale(bounds.Width, bounds.Height, 1.0f);
        matrixStack.Push();
        matrixStack.Translate(0.5f, 0.5f, 0.0f);
        VertexDrawData vertexDrawData = new VertexDrawData(mesh.VertexArray, mesh.Count, matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection, null, Color4.White, PrimitiveType.LineLoop);
        renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
        matrixStack.Pop();
    }
}