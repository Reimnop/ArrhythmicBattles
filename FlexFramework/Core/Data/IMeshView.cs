﻿namespace FlexFramework.Core.Data;

public interface IMeshView
{
    IBufferView VertexBuffer { get; }
    IBufferView? IndexBuffer { get; }
    int VerticesCount { get; }
    int IndicesCount { get; }
    int VertexSize { get; }
    VertexLayout VertexLayout { get; }
}