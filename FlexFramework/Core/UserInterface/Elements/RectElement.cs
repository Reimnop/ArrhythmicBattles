using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpFont;

namespace FlexFramework.Core.UserInterface.Elements;

public class RectElement : VisualElement, IRenderable, IDisposable
{
    public float Radius { get; set; } = 0.0f;
    public Color4 Color { get; set; } = Color4.White;

    private readonly Mesh<Vertex> mesh = new Mesh<Vertex>("rect");
    private readonly List<Vector2> vertexPositions = new List<Vector2>();

    public RectElement(params Element[] children) : base(children)
    {
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
        
        vertexPositions.Clear();
        MeshGenerator.GenerateRoundedRectangle(vertexPositions, ElementBounds.Min, ElementBounds.Max, Radius);
        
        Span<Vertex> vertices = stackalloc Vertex[vertexPositions.Count];
        for (int i = 0; i < vertexPositions.Count; i++)
        {
            Vector2 pos = vertexPositions[i];
            Vector2 relativePos = pos - ElementBounds.Min;
            Vector2 uv = new Vector2(relativePos.X / ElementBounds.Width, relativePos.Y / ElementBounds.Height);
            vertices[i] = new Vertex(new Vector3(pos), uv);
        }
        
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
    }

    public void Dispose()
    {
        mesh.Dispose();
    }
}