using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Renderables;

public class BoundingBoxRenderable : IRenderable
{
    private readonly Mesh<Vertex> mesh;
    private readonly Bounds bounds;
    private readonly Color4 color;
    
    public BoundingBoxRenderable(FlexFrameworkMain engine, Bounds bounds, Color4 color)
    {
        this.bounds = bounds;
        this.color = color;
        
        EngineResources resources = engine.Resources;
        mesh = engine.ResourceManager.GetResource<Mesh<Vertex>>(resources.QuadWireframeMesh);
    }
    
    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        matrixStack.Translate(0.5f, 0.5f, 0.0f);
        matrixStack.Scale(bounds.Width, bounds.Height, 1.0f);
        matrixStack.Translate(bounds.X0, bounds.Y0, 0.0f);
        VertexDrawData vertexDrawData = new VertexDrawData(mesh.VertexArray, mesh.Count, matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection, null, color, PrimitiveType.LineLoop);
        renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
    }
}