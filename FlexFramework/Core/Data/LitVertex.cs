using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

public struct LitVertex : IVertex
{
    public Vector3 Position { get; set; }
    public Vector3 Normal { get; set; }
    public Vector2 Uv { get; set; }
    public Color4 Color { get; set; }

    public LitVertex(Vector3 position, Vector3 normal, Vector2 uv, Color4 color)
    {
        Position = position;
        Normal = normal;
        Uv = uv;
        Color = color;
    }
    
    public LitVertex(float x, float y, float z, float nx, float ny, float nz, float u, float v, float r, float g, float b, float a)
    {
        Position = new Vector3(x, y, z);
        Normal = new Vector3(nx, ny, nz);
        Uv = new Vector2(u, v);
        Color = new Color4(r, g, b, a);
    }

    public static void SetupAttributes(VertexAttributeConsumer attribConsumer, VertexAttributeIConsumer intAttribConsumer)
    {
        attribConsumer(0, 3, 0, VertexAttribType.Float, false);
        attribConsumer(1, 3, 3 * sizeof(float), VertexAttribType.Float, false);
        attribConsumer(2, 2, 6 * sizeof(float), VertexAttribType.Float, false);
        attribConsumer(3, 4, 8 * sizeof(float), VertexAttribType.Float, false);
    }
}