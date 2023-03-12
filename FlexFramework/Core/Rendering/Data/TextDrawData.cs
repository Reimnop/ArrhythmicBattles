using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct TextDrawData : IDrawData
{
    public VertexArray VertexArray { get; }
    public int Count { get; }
    public Matrix4 Transformation { get; }
    public Color4 Color { get; }
    public float DistanceRange { get; }

    public TextDrawData(VertexArray vertexArray, int count, Matrix4 transformation, Color4 color, float distanceRange)
    {
        VertexArray = vertexArray;
        Count = count;
        Transformation = transformation;
        Color = color;
        DistanceRange = distanceRange;
    }
}