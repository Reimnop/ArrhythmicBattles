using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class RectElement : VisualElement, IRenderable, IDisposable
{
    public float Radius { get; set; } = 0.0f;
    public Color4 Color { get; set; } = Color4.White;

    private Mesh<Vertex> mesh = new Mesh<Vertex>("rect");

    public RectElement(params Element[] children) : base(children)
    {
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
        
        // Render self
        Vector2[] vertexPositions = MeshGenerator.GenerateRoundedRectangle(ElementBounds.Min, ElementBounds.Max, Radius);
        Vertex[] vertices = vertexPositions
            .Select(pos =>
            {
                Vector2 relativePos = pos - ElementBounds.Min;
                Vector2 uv = new Vector2(relativePos.X / ElementBounds.Width, relativePos.Y / ElementBounds.Height);
                return new Vertex(new Vector3(pos), uv);
            })
            .ToArray();
        mesh.LoadData(vertices);
    }

    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        CameraData cameraData = args.CameraData;
        int layerId = args.LayerId;
        
        matrixStack.Push();
        VertexDrawData vertexDrawData = new VertexDrawData(mesh.VertexArray, mesh.Count, matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection, null, Color, PrimitiveType.Triangles);
        args.Renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
        
        DrawDebugBoxes(args);
    }

    public void Dispose()
    {
        mesh.Dispose();
    }
}