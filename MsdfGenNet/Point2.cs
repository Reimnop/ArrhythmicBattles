using System.Runtime.InteropServices;

namespace MsdfGenNet;

[StructLayout(LayoutKind.Sequential)]
public struct Point2
{
    public double X
    {
        get => x;
        set => x = value;
    }

    public double Y
    {
        get => y;
        set => y = value;
    }

    private double x, y;
    
    public Point2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
}