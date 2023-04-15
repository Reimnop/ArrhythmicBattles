using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct LitVertexDrawData : IDrawData
{
    public Mesh<LitVertex> Mesh { get; }
    public Matrix4 ModelMatrix { get; }
    public Matrix4 Transformation { get; }
    public Texture2D? Texture { get; }
    public Color4 Color { get; }

    public LitVertexDrawData(Mesh<LitVertex> mesh, Matrix4 modelMatrix, Matrix4 transformation, Texture2D? texture, Color4 color)
    {
        Mesh = mesh;
        ModelMatrix = modelMatrix;
        Transformation = transformation;
        Texture = texture;
        Color = color;
    }
}