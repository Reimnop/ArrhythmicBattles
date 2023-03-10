using MsdfGenNet.Native;

namespace MsdfGenNet;

// Wrapper for EdgeHolder, but simplified to just Edge
public class Edge : IDisposable
{
    internal IntPtr Handle { get; private set; }
    
    public Edge()
    {
        Handle = MsdfGenNative.msdfgen_EdgeHolder_new();
    }
    
    public Edge(Point2 p0, Point2 p1, EdgeColor edgeColor)
    {
        Handle = MsdfGenNative.msdfgen_EdgeHolder_newLinear(p0, p1, edgeColor);
    }
    
    public Edge(Point2 p0, Point2 p1, Point2 p2, EdgeColor edgeColor)
    {
        Handle = MsdfGenNative.msdfgen_EdgeHolder_newQuadratic(p0, p1, p2, edgeColor);
    }
    
    public Edge(Point2 p0, Point2 p1, Point2 p2, Point2 p3, EdgeColor edgeColor)
    {
        Handle = MsdfGenNative.msdfgen_EdgeHolder_newCubic(p0, p1, p2, p3, edgeColor);
    }

    public Edge(Edge edge)
    {
        Handle = MsdfGenNative.msdfgen_EdgeHolder_newClone(edge.Handle);
    }
    
    internal Edge(IntPtr handle)
    {
        Handle = handle;
    }

    ~Edge()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            MsdfGenNative.msdfgen_EdgeHolder_free(Handle);
            Handle = IntPtr.Zero;
        }
        GC.SuppressFinalize(this);
    }
}