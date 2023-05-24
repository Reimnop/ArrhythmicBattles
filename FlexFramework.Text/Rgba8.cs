using System.Runtime.InteropServices;

namespace FlexFramework.Text;

/// <summary>
/// Represents a color with red, green, blue, and alpha components.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rgba8
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;
    
    public Rgba8(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
}