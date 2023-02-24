using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class RectEntity : Entity, IRenderable, IDisposable
{
    public Vector2 Min
    {
        get => min;
        set
        {
            min = value;
            InvalidateMesh();
        }
    }
    
    public Vector2 Max
    {
        get => max;
        set
        {
            max = value;
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

    private Vector2 min;
    private Vector2 max;
    private float radius;

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
        float width = max.X - min.X;
        float height = max.Y - min.Y;
        
        if (width * height == 0)
        {
            return;
        }
        
        vertexPositions.Clear();
        MeshGenerator.GenerateRoundedRectangle(vertexPositions, Min, Max, Radius);
        
        Span<Vertex> vertices = stackalloc Vertex[vertexPositions.Count];
        for (int i = 0; i < vertexPositions.Count; i++)
        {
            Vector2 pos = vertexPositions[i];
            Vector2 relativePos = pos - Min;
            Vector2 uv = new Vector2(relativePos.X / width, relativePos.Y / height);
            vertices[i] = new Vertex(new Vector3(pos), uv);
        }
        
        mesh.LoadData(vertices);
    }

    public void Render(RenderArgs args)
    {
        if (!meshValid)
        {
            meshValid = true;
            GenerateMesh();
        }
        
        if (mesh.Count == 0)
        {
            return;
        }
        
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