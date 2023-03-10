using System.Runtime.InteropServices;

using static MsdfGenNet.Native.Constants;

namespace MsdfGenNet.Native;

// msdfgen_wrap.h
internal static class MsdfGenNative
{
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_generateSDF(IntPtr output, IntPtr shape, IntPtr projection, double range, IntPtr config);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_generatePseudoSDF(IntPtr output, IntPtr shape, IntPtr projection, double range, IntPtr config);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_generateMSDF(IntPtr output, IntPtr shape, IntPtr projection, double range, IntPtr config);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_generateMTSDF(IntPtr output, IntPtr shape, IntPtr projection, double range, IntPtr config);
    
    // Shape
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Shape_new();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_free(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_addContour(IntPtr shape, IntPtr contour);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Shape_addContourNew(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_normalize(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool msdfgen_Shape_validate(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_bound(IntPtr shape, ref double l, ref double b, ref double r, ref double t);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_boundMiters(IntPtr shape, ref double l, ref double b, ref double r, ref double t, double border, double miterLimit, int polarity);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Bounds msdfgen_Shape_getBounds(IntPtr shape);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_scanline(IntPtr shape, IntPtr line, double y);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int msdfgen_Shape_edgeCount(IntPtr shape);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Shape_orientContours(IntPtr shape);
    
    // Contour
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Contour_new();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_free(IntPtr contour);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_addEdge(IntPtr contour, IntPtr edge);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_Contour_addEdgeNew(IntPtr contour);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_bound(IntPtr contour, ref double l, ref double b, ref double r, ref double t);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_boundMiters(IntPtr contour, ref double l, ref double b, ref double r, ref double t, double border, double miterLimit, int polarity);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int msdfgen_Contour_winding(IntPtr contour);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_Contour_reverse(IntPtr contour);
    
    // EdgeHolder
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_new();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newFromEdgeSegment(IntPtr edgeSegment); // hell no
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newLinear(Point2 p0, Point2 p1, EdgeColor edgeColor);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newQuadratic(Point2 p0, Point2 p1, Point2 p2, EdgeColor edgeColor);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newCubic(Point2 p0, Point2 p1, Point2 p2, Point2 p3, EdgeColor edgeColor);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr msdfgen_EdgeHolder_newClone(IntPtr edge);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void msdfgen_EdgeHolder_free(IntPtr edge);
}