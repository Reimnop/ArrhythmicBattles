using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using Buffer = FlexFramework.Core.Rendering.Data.Buffer;

namespace FlexFramework.Core.Data;

public class Mesh<T> : IDisposable where T : unmanaged, IVertex
{
    public VertexArray VertexArray { get; }
    public Buffer VertexBuffer { get; }
    public int Count { get; private set; }

    public Mesh(string name)
    {
        VertexBuffer = new Buffer($"{name}-vtx");
        VertexArray = new VertexArray(name);
        
        T.SetupAttributes(Attribute, AttributeI);
    }

    public Mesh(string name, ReadOnlySpan<T> vertices)
    {
        VertexBuffer = new Buffer($"{name}-vtx");
        VertexArray = new VertexArray(name);
        
        T.SetupAttributes(Attribute, AttributeI);
        
        LoadData(vertices);
    }

    public void LoadData(ReadOnlySpan<T> vertices)
    {
        Count = vertices.Length;

        if (VertexBuffer.SizeInBytes >= vertices.Length * Unsafe.SizeOf<T>())
        {
            VertexBuffer.LoadDataPartial(vertices, 0);
        }
        else
        {
            VertexBuffer.LoadData(vertices);
        }
    }

    private void Attribute(int index, int size, int offset, VertexAttribType vertexAttribType, bool normalized)
    {
        VertexArray.VertexBuffer(VertexBuffer, index, index, size, offset, vertexAttribType, normalized, Unsafe.SizeOf<T>());
    }
    
    private void AttributeI(int index, int size, int offset, VertexAttribIntegerType vertexAttribIntegerType)
    {
        VertexArray.VertexBuffer(VertexBuffer, index, index, size, offset, vertexAttribIntegerType, Unsafe.SizeOf<T>());
    }

    public void Dispose()
    {
        VertexArray.Dispose();
        VertexBuffer.Dispose();
    }
}