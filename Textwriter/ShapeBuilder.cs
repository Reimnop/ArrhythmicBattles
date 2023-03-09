using Msdfgen;
using SharpFont;

namespace Textwriter;

public class ShapeBuilder
{
    public Shape Shape => shape;
    
    private readonly Shape shape;
    private Contour currentContour;
    private Vector2 lastPoint;

    public ShapeBuilder(Outline outline)
    {
        shape = new Shape();
        
        OutlineFuncs funcs = new OutlineFuncs();
        funcs.MoveFunction = MoveTo;
        funcs.LineFunction = LineTo;
        funcs.ConicFunction = ConicTo;
        funcs.CubicFunction = CubicTo;
        funcs.Shift = 0;
        
        outline.Decompose(funcs, IntPtr.Zero);
    }

    private Vector2 FromFtVector(ref FTVector vector)
    {
        return new Vector2(vector.X.Value / 64.0, vector.Y.Value / 64.0);
    }
    
    private int MoveTo(ref FTVector to, IntPtr context)
    {
        currentContour = new Contour();
        shape.Add(currentContour);
        lastPoint = FromFtVector(ref to);
        return 0;
    }

    private int LineTo(ref FTVector to, IntPtr context)
    {
        currentContour.Add(new LinearSegment(lastPoint, FromFtVector(ref to)));
        lastPoint = FromFtVector(ref to);
        return 0;
    }

    private int ConicTo(ref FTVector control, ref FTVector to, IntPtr context)
    {
        currentContour.Add(new QuadraticSegment(EdgeColor.White, lastPoint, FromFtVector(ref control), FromFtVector(ref to)));
        lastPoint = FromFtVector(ref to);
        return 0;
    }

    private int CubicTo(ref FTVector control1, ref FTVector control2, ref FTVector to, IntPtr context)
    {
        currentContour.Add(new CubicSegment(EdgeColor.White, lastPoint, FromFtVector(ref control1), FromFtVector(ref control2),FromFtVector(ref to)));
        lastPoint = FromFtVector(ref to);
        return 0;
    }
}