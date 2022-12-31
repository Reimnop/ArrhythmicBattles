using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Text;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core;

public class PersistentResources : IDisposable
{
    public Mesh<Vertex> QuadMesh { get; }

    public PersistentResources()
    {
        Vertex[] quadVertices =
        {
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, -0.5f, 0.0f, 1.0f, 0.0f)
        };
        
        QuadMesh = new Mesh<Vertex>("quad", quadVertices);
    }
    
    public void Dispose()
    {
        QuadMesh.Dispose();
    }
}