using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class RectEntity : Entity, IRenderable
{
    public Box2 Bounds
    {
        get => bounds;
        set
        {
            bounds = value;
            InvalidateMesh();
        }
    }

    public float Radius
    {
        get => radius;
        set
        {
            radius = value;
            InvalidateMesh();
        }
    }
    
    public Color4 Color { get; set; } = Color4.White;

    private Box2 bounds;
    private float radius;
    
    private Box2 lastBounds;

    private bool meshValid = false;
    
    private readonly Mesh<Vertex> mesh;
    private readonly List<Vector2> vertexPositions = new List<Vector2>();

    public RectEntity()
    {
        mesh = new Mesh<Vertex>("rect");
    }

    public void InvalidateMesh()
    {
        meshValid = false;
    }

    private void GenerateMesh()
    {
        var size = bounds.Size;
        if (size.X * size.Y == 0)
            return;
        
        if (bounds == lastBounds)
            return;
        lastBounds = bounds;

        vertexPositions.Clear();
        MeshGenerator.GenerateRoundedRectangle(vertexPositions, bounds.Min, bounds.Max, Radius);

        Span<Vertex> vertices = stackalloc Vertex[vertexPositions.Count];
        for (int i = 0; i < vertexPositions.Count; i++)
        {
            var pos = vertexPositions[i];
            var u = (pos.X - bounds.Min.X) / size.X;
            var v = (pos.Y - bounds.Min.Y) / size.Y;
            vertices[i] = new Vertex(pos.X, pos.Y, 0.0f, u, v);
        }

        mesh.SetData(vertices, null);
    }

    public void Render(RenderArgs args)
    {
        var size = bounds.Size;
        if (size.X * size.Y == 0)
        {
            return;
        }

        if (!meshValid)
        {
            meshValid = true;
            GenerateMesh();
        }

        var commandList = args.CommandList;
        var layerType = args.LayerType;
        var matrixStack = args.MatrixStack;
        var cameraData = args.CameraData;

        matrixStack.Push();
        VertexDrawData vertexDrawData = new VertexDrawData(mesh.ReadOnly, matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection, null, Color, PrimitiveType.Triangles);
        commandList.AddDrawData(layerType, vertexDrawData);
        matrixStack.Pop();
    }
}