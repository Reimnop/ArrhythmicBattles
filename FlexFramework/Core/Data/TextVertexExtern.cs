using OpenTK.Graphics.OpenGL4;
using Textwriter;

namespace FlexFramework.Core.Data;

public struct TextVertexExtern : IVertex
{
    public TextVertex TextVertex { get; set; }

    public TextVertexExtern(TextVertex textVertex)
    {
        TextVertex = textVertex;
    }

    public static void SetupAttributes(VertexAttributeConsumer attribConsumer, VertexAttributeIConsumer intAttribConsumer)
    {
        attribConsumer(0, 2, 0, VertexAttribType.Float, false);
        attribConsumer(1,4, 2 * sizeof(float), VertexAttribType.Float, false);
        attribConsumer(2, 2, 6 * sizeof(float), VertexAttribType.Float, false);
        intAttribConsumer(3, 1, 8 * sizeof(float), VertexAttribIntegerType.Int);
        intAttribConsumer(4, 1, 9 * sizeof(float), VertexAttribIntegerType.Int);
    }
}