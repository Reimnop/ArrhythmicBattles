using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

public struct BoneWeight
{
    public int Index { get; }
    public float Weight { get; }

    public BoneWeight(int index, float weight)
    {
        Index = index;
        Weight = weight;
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SkinnedVertex : IVertex
{
    public const int MaxBoneInfluence = 4;
    
    public Vector3 Position { get; set; }
    public Vector2 Uv { get; set; }
    public Color4 Color { get; set; }

    public fixed int BoneIndices[MaxBoneInfluence];
    public fixed float Weights[MaxBoneInfluence];

    public SkinnedVertex(Vector3 position, Vector2 uv, Color4 color, params BoneWeight[] weights)
    {
        Position = position;
        Uv = uv;
        Color = color;

        if (weights.Length > MaxBoneInfluence)
        {
            throw new IndexOutOfRangeException();
        }

        for (int i = 0; i < weights.Length; i++)
        {
            BoneIndices[i] = weights[i].Index;
            Weights[i] = weights[i].Weight;
        }
        
        for (int i = weights.Length; i < MaxBoneInfluence; i++)
        {
            BoneIndices[i] = -1;
            Weights[i] = 0.0f;
        }
    }
    
    public SkinnedVertex(float x, float y, float z, float u, float v, float r, float g, float b, float a, params BoneWeight[] weights)
    {
        Position = new Vector3(x, y, z);
        Uv = new Vector2(u, v);
        Color = new Color4(r, g, b, a);
        
        if (weights.Length > MaxBoneInfluence)
        {
            throw new IndexOutOfRangeException();
        }

        for (int i = 0; i < weights.Length; i++)
        {
            BoneIndices[i] = weights[i].Index;
            Weights[i] = weights[i].Weight;
        }

        for (int i = weights.Length; i < MaxBoneInfluence; i++)
        {
            BoneIndices[i] = -1;
            Weights[i] = 0.0f;
        }
    }

    public SkinnedVertex AppendWeight(BoneWeight weight)
    {
        SkinnedVertex newVertex = this;
        
        for (int i = 0; i < MaxBoneInfluence; i++)
        {
            if (newVertex.BoneIndices[i] == -1)
            {
                newVertex.BoneIndices[i] = weight.Index;
                newVertex.Weights[i] = weight.Weight;
                return newVertex;
            }
        }

        return newVertex;
    }

    public static void SetupAttributes(VertexAttributeConsumer attribConsumer, VertexAttributeIConsumer intAttribConsumer)
    {
        attribConsumer(0, 3, 0, VertexAttribType.Float, false);
        attribConsumer(1, 2, 3 * sizeof(float), VertexAttribType.Float, false);
        attribConsumer(2, 4, 5 * sizeof(float), VertexAttribType.Float, false);
        intAttribConsumer(3, MaxBoneInfluence, 9 * sizeof(float), VertexAttribIntegerType.Int);
        attribConsumer(4, MaxBoneInfluence, 9 * sizeof(float) + MaxBoneInfluence * sizeof(int), VertexAttribType.Float, false);
    }
}