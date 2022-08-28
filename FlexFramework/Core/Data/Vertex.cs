using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex : IVertex
{
    public Vector3 Position { get; set; }
    public Vector2 Uv { get; set; }
    public Color4 Color { get; set; }

    public Vertex(Vector3 position, Vector2 uv)
    {
        Position = position;
        Uv = uv;
        Color = Color4.White;
    }
    
    public Vertex(float x, float y, float z, float u, float v)
    {
        Position = new Vector3(x, y, z);
        Uv = new Vector2(u, v);
        Color = Color4.White;
    }
    
    public Vertex(Vector3 position, Vector2 uv, Color4 color)
    {
        Position = position;
        Uv = uv;
        Color = color;
    }
    
    public Vertex(float x, float y, float z, float u, float v, float r, float g, float b, float a)
    {
        Position = new Vector3(x, y, z);
        Uv = new Vector2(u, v);
        Color = new Color4(r, g, b, a);
    }

    public static void SetupAttributes(VertexAttributeConsumer attribConsumer, VertexAttributeIConsumer intAttribConsumer)
    {
        attribConsumer(0, 3, 0, VertexAttribType.Float, false);
        attribConsumer(1, 2, 3 * sizeof(float), VertexAttribType.Float, false);
        attribConsumer(2, 4, 5 * sizeof(float), VertexAttribType.Float, false);
    }
}