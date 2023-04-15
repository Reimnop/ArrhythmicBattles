namespace FlexFramework.Core.Data;

public interface IMeshView
{
    Buffer VertexBuffer { get; }
    Buffer? IndexBuffer { get; }
    int VerticesCount { get; }
    int IndicesCount { get; }
    int VertexSize { get; }
}