﻿using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Data;

public struct TextDrawData : IDrawData
{
    public Mesh<TextVertexAdapter> Mesh { get; }
    public int Count { get; }
    public Matrix4 Transformation { get; }
    public Color4 Color { get; }
    public float DistanceRange { get; }

    public TextDrawData(Mesh<TextVertexAdapter> mesh, int count, Matrix4 transformation, Color4 color, float distanceRange)
    {
        Mesh = mesh;
        Count = count;
        Transformation = transformation;
        Color = color;
        DistanceRange = distanceRange;
    }
}