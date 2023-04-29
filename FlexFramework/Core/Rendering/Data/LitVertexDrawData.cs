using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct LitVertexDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public Matrix4 ModelMatrix { get; }
    public Matrix4 Transformation { get; }
    public ITextureView? Texture { get; }
    public Color4 Color { get; }

    public LitVertexDrawData(IMeshView mesh, Matrix4 modelMatrix, Matrix4 transformation, ITextureView? texture, Color4 color)
    {
        Mesh = mesh;
        ModelMatrix = modelMatrix;
        Transformation = transformation;
        Texture = texture;
        Color = color;
    }
}