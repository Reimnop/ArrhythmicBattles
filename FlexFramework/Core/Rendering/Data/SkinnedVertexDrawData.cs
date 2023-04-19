using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct SkinnedVertexDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public Matrix4 ModelMatrix { get; }
    public Matrix4 Transformation { get; }
    public Texture2D? Texture { get; }
    public Matrix4[]? Bones { get; }
    public Color4 Color { get; }

    public SkinnedVertexDrawData(IMeshView mesh, Matrix4 modelMatrix, Matrix4 transformation, Texture2D? texture, Matrix4[]? bones, Color4 color)
    {
        Mesh = mesh;
        ModelMatrix = modelMatrix;
        Transformation = transformation;
        Texture = texture;
        Bones = bones;
        Color = color;
    }
}