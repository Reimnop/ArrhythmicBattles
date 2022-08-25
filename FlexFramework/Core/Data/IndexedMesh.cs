using System.Runtime.CompilerServices;
using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using Buffer = FlexFramework.Rendering.Data.Buffer;

namespace FlexFramework.Core.Data;

public class IndexedMesh<T> : IDisposable where T : struct
{
    public VertexArray VertexArray { get; }
    public Buffer VertexBuffer { get; }
    public Buffer IndexBuffer { get; }
    public int Count { get; private set; }

    private int currentAttribIndex = 0;
    
    public IndexedMesh(string name)
    {
        VertexBuffer = new Buffer($"{name}-vtx");
        IndexBuffer = new Buffer($"{name}-idx");
        VertexArray = new VertexArray(name);
    }

    public IndexedMesh(string name, T[] vertices, int[] indices)
    {
        VertexBuffer = new Buffer($"{name}-vtx");
        IndexBuffer = new Buffer($"{name}-idx");
        
        VertexArray = new VertexArray(name);
        VertexArray.ElementBuffer(IndexBuffer);
        
        LoadData(vertices, indices);
    }

    public void LoadData(T[] vertices, int[] indices)
    {
        Count = indices.Length;

        if (VertexBuffer.SizeInBytes >= vertices.Length * Unsafe.SizeOf<T>())
        {
            VertexBuffer.LoadDataPartial(vertices, 0);
        }
        else
        {
            VertexBuffer.LoadData(vertices);
        }
        
        if (IndexBuffer.SizeInBytes >= indices.Length * sizeof(int))
        {
            IndexBuffer.LoadDataPartial(vertices, 0);
        }
        else
        {
            IndexBuffer.LoadData(indices);
        }
    }

    public void Attribute(int size, int offset, VertexAttribType vertexAttribType, bool normalized)
    {
        VertexArray.VertexBuffer(VertexBuffer, currentAttribIndex, currentAttribIndex, size, offset, vertexAttribType, normalized, Unsafe.SizeOf<T>());
        currentAttribIndex++;
    }
    
    public void Attribute(int size, int offset, VertexAttribIntegerType vertexAttribIntegerType)
    {
        VertexArray.VertexBuffer(VertexBuffer, currentAttribIndex, currentAttribIndex, size, offset, vertexAttribIntegerType, Unsafe.SizeOf<T>());
        currentAttribIndex++;
    }

    public void Dispose()
    {
        VertexArray.Dispose();
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}