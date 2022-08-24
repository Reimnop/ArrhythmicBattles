using OpenTK.Mathematics;

namespace FlexFramework.Rendering.Data;

public struct TextDrawData : IDrawData
{
    public VertexArray VertexArray { get; }
    public int Count { get; }
    public Matrix4 Transformation { get; }
    public Color4 Color { get; }

    public TextDrawData(VertexArray vertexArray, int count, Matrix4 transformation, Color4 color)
    {
        VertexArray = vertexArray;
        Count = count;
        Transformation = transformation;
        Color = color;
    }
}