namespace FlexFramework.Core.Data;

public interface IMeshView
{
    string Name { get; }
    IBufferView VertexBuffer { get; }
    IBufferView? IndexBuffer { get; }
    int VerticesCount { get; }
    int IndicesCount { get; }
    int VertexSize { get; }
    VertexLayout VertexLayout { get; }
}