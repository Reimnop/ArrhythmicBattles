using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Data;

public delegate void VertexAttributeConsumer(int index, int size, int offset, VertexAttribType vertexAttribType, bool normalized);
public delegate void VertexAttributeIConsumer(int index, int size, int offset, VertexAttribIntegerType vertexAttribIntegerType);

public interface IVertex
{
    static abstract void SetupAttributes(VertexAttributeConsumer attribConsumer, VertexAttributeIConsumer intAttribConsumer);
}