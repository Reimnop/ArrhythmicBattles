using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct SkinnedVertexDrawData : IDrawData
{
    public VertexArray VertexArray { get; }
    public int Count { get; }
    public Matrix4 ModelMatrix { get; }
    public Matrix4 Transformation { get; }
    public Texture2D? Texture { get; }
    public Matrix4[]? Bones { get; }
    public Color4 Color { get; }

    public SkinnedVertexDrawData(VertexArray vertexArray, int count, Matrix4 modelMatrix, Matrix4 transformation, Texture2D? texture, Matrix4[]? bones, Color4 color)
    {
        VertexArray = vertexArray;
        Count = count;
        ModelMatrix = modelMatrix;
        Transformation = transformation;
        Texture = texture;
        Bones = bones;
        Color = color;
    }
}