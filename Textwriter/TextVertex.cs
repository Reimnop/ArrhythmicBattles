using System.Numerics;
using System.Runtime.InteropServices;

namespace Textwriter;

[StructLayout(LayoutKind.Sequential)]
public struct TextVertex
{
    public Vector2 Position;
    public Vector4 Color;
    public Vector2 Uv;
    public int Colored;
    public int TextureIndex;

    public TextVertex(float x, float y, float u, float v, float r, float g, float b, float a, bool colored, int index)
    {
        Position = new Vector2(x, y);
        Uv = new Vector2(u, v);
        Color = new Vector4(r, g, b, a);
        Colored = colored ? 1 : 0;
        TextureIndex = index;
    }

    public TextVertex(float x, float y, float r, float g, float b, float a) : this(x, y, 0.0f, 0.0f, r, g, b, a, false, -1) // -1 is reserved for no texture
    {
    }
}