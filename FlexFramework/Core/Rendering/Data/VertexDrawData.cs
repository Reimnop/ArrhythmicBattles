﻿using FlexFramework.Core.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct VertexDrawData : IDrawData
{
    public IMeshView Mesh { get; }
    public Matrix4 Transformation { get; }
    public Texture2D? Texture { get; }
    public Color4 Color { get; }
    public PrimitiveType PrimitiveType { get; }

    public VertexDrawData(IMeshView mesh, Matrix4 transformation, Texture2D? texture, Color4 color, PrimitiveType primitiveType)
    {
        Mesh = mesh;
        Transformation = transformation;
        Texture = texture;
        Color = color;
        PrimitiveType = primitiveType;
    }
}