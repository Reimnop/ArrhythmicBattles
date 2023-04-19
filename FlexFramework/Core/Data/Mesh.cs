using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FlexFramework.Core.Data;

public class Mesh<T> : DataObject where T : unmanaged
{
    private struct ReadOnlyMesh : IMeshView
    {
        public IBufferView VertexBuffer => mesh.VertexBuffer.AsReadOnly();
        public IBufferView? IndexBuffer => mesh.IndexBuffer?.AsReadOnly();
        public int VerticesCount => mesh.VerticesCount;
        public int IndicesCount => mesh.IndicesCount;
        public int VertexSize => mesh.VertexSize;
        public VertexLayout VertexLayout => mesh.VertexLayout;

        private readonly Mesh<T> mesh;
        
        public ReadOnlyMesh(Mesh<T> mesh)
        {
            this.mesh = mesh;
        }
    }
    
    public Buffer VertexBuffer => vertexBuffer;
    public Buffer? IndexBuffer => indexBuffer;
    public int VerticesCount => verticesCount;
    public int IndicesCount => indicesCount;
    public int VertexSize => Unsafe.SizeOf<T>();
    public VertexLayout VertexLayout { get; }

    private readonly Buffer vertexBuffer = new Buffer();
    private Buffer? indexBuffer = null;
    private int verticesCount = 0;
    private int indicesCount = 0;

    public Mesh(string name, VertexLayout? vertexLayout = null) : base(name)
    {
        // If vertex layout is not specified, generate it from field attributes
        VertexLayout = vertexLayout ?? VertexLayout.GetLayout<T>();
    }

    public Mesh(string name, ReadOnlySpan<T> vertices, VertexLayout? vertexLayout = null) : this(name, vertexLayout)
    {
        SetData(vertices, null);
    }
    
    public Mesh(string name, ReadOnlySpan<T> vertices, ReadOnlySpan<int> indices, VertexLayout? vertexLayout = null) : this(name, vertexLayout)
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

    public IMeshView AsReadOnly()
    {
        return new ReadOnlyMesh(this);
    }
}