using OpenTK.Mathematics;

namespace FlexFramework.Rendering.Data;

public struct VertexDrawData : IDrawData
{
    public VertexArray VertexArray { get; }
    public int Count { get; }
    public Matrix4 Transformation { get; }
    public Texture2D? Texture { get; }
    public Color4 Color { get; }

    public VertexDrawData(VertexArray vertexArray, int count, Matrix4 transformation, Texture2D? texture, Color4 color)
    {
        VertexArray = vertexArray;
        Count = count;
        Transformation = transformation;
        Texture = texture;
        Color = color;
    }
}