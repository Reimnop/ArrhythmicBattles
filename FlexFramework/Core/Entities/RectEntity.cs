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
    
    private Vector2 lastSize;

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
        Vector2 size = max - min;
        
        if (size.X * size.Y == 0)
        {
            return;
        }

        if (size == lastSize)
        {
            return;
        }
        
        vertexPositions.Clear();
        MeshGenerator.GenerateRoundedRectangle(vertexPositions, Vector2.Zero, size, Radius);

        Span<Vertex> vertices = stackalloc Vertex[vertexPositions.Count];
        for (int i = 0; i < vertexPositions.Count; i++)
        {
            Vector2 pos = vertexPositions[i];
            Vector2 uv = new Vector2(pos.X / size.X, pos.Y / size.Y);
            vertices[i] = new Vertex(new Vector3(pos), uv);
        }

        mesh.LoadData(vertices);
        
        lastSize = size;
    }

    public void Render(RenderArgs args)
    {
        Vector2 size = max - min;
        
        if (size.X * size.Y == 0)
        {
            return;
        }

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
        matrixStack.Translate(Min.X, Min.Y, 0.0f);
        VertexDrawData vertexDrawData = new VertexDrawData(mesh.VertexArray, mesh.Count, matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection, null, Color, PrimitiveType.Triangles);
        args.Renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
    }
    
    public void Dispose()
    {
        mesh.Dispose();
    }
}