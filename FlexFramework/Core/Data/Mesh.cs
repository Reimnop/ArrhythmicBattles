using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FlexFramework.Core.Data;

public class Mesh<T> : DataObject, IMeshView where T : unmanaged, IVertex
{
    public Buffer VertexBuffer => vertexBuffer;
    public Buffer? IndexBuffer => indexBuffer;
    public int VerticesCount => verticesCount;
    public int IndicesCount => indicesCount;
    public int VertexSize => Unsafe.SizeOf<T>();

    private readonly Buffer vertexBuffer = new Buffer();
    private Buffer? indexBuffer = null;
    private int verticesCount = 0;
    private int indicesCount = 0;

    public Mesh(string name) : base(name)
    {
    }

    public Mesh(string name, ReadOnlySpan<T> vertices) : this(name)
    {
        SetData(vertices, null);
    }
    
    public Mesh(string name, ReadOnlySpan<T> vertices, ReadOnlySpan<int> indices) : this(name)
    {
        SetData(vertices, indices);
    }

    public void SetData(ReadOnlySpan<T> vertices, ReadOnlySpan<int> indices)
    {
        vertexBuffer.SetData(vertices);
        verticesCount = vertices.Length;
        
        if (indices.Length > 0)
        {
            indexBuffer = new Buffer();
            indexBuffer.SetData(indices);
            indicesCount = indices.Length;
        }
        else
        {
            indexBuffer = null;
        }
    }
    
    public T GetVertex(int index)
    {
        if (index < 0 || index >= verticesCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        
        int size = Unsafe.SizeOf<T>();
        ReadOnlySpan<byte> data = vertexBuffer.Data;
        return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(data.Slice(index * size, size)));
    }
    
    public int GetIndex(int index)
    {
        if (index < 0 || index >= indicesCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        
        ReadOnlySpan<byte> data = indexBuffer!.Data;
        return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(data.Slice(index * sizeof(int), sizeof(int))));
    }
    
    public void SetVertex(int index, T vertex)
    {
        if (index < 0 || index >= verticesCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        
        int size = Unsafe.SizeOf<T>();
        Span<byte> data = vertexBuffer.Data;
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(data.Slice(index * size, size)), vertex);
    }
    
    public void SetIndex(int index, int value)
    {
        if (index < 0 || index >= indicesCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        
        Span<byte> data = indexBuffer!.Data;
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(data.Slice(index * sizeof(int), sizeof(int))), value);
    }
}