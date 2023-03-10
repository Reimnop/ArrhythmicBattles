namespace Msdfgen;

/// Vector shape representation.
public class Shape : List<Contour>
{
    // Threshold of the dot product of adjacent edge directions to be considered convergent.
    private const double MsdfgenCornerDotEpsilon = .000001;
    // The proportional amount by which a curve's control point will be adjusted to eliminate convergent corners.
    private const double MsdfgenDeconvergenceFactor = .000001;
    
    /// Specifies whether the shape uses bottom-to-top (false) or top-to-bottom (true) Y coordinates.
    public bool InverseYAxis = false;

    /// Normalizes the shape geometry for distance field generation.
    public void Normalize()
    {
        foreach (var contour in this)
            if (contour.Count == 1)
            {
                var parts = new EdgeSegment[3];
                contour[0].SplitInThirds(out parts[0], out parts[1], out parts[2]);
                contour.Clear();
                contour.Add(parts[0]);
                contour.Add(parts[1]);
                contour.Add(parts[2]);
            }
            else
            {
                EdgeSegment prevEdge = contour[^1];
                foreach (EdgeSegment edge in contour)
                {
                    Vector2 prevDir = prevEdge.Direction(1).Normalize();
                    Vector2 curDir = edge.Direction(0).Normalize();
                    if (Vector2.Dot(prevDir, curDir) < MsdfgenCornerDotEpsilon - 1.0)
                    {
                        DeconvergeEdge(prevEdge, 1);
                        DeconvergeEdge(edge, 0);
                    }

                    prevEdge = edge;
                }
            }
    }
    
    private void DeconvergeEdge(EdgeSegment edgeSegment, int param) 
    {
        QuadraticSegment? quadraticSegment = edgeSegment as QuadraticSegment;
        if (quadraticSegment != null)
            edgeSegment = quadraticSegment.ToCubic();
        
        CubicSegment? cubicSegment = edgeSegment as CubicSegment;
        if (cubicSegment != null)
            cubicSegment.Deconverge(param, MsdfgenDeconvergenceFactor);
    }

    /// Performs basic checks to determine if the object represents a valid shape.
    public bool Validate()
    {
        foreach (var contour in this)
            if (contour.Count > 0)
            {
                var corner = contour[^1].Point(1);
                foreach (var edge in contour)
                {
                    if (edge == null)
                        return false;
                    if (edge.Point(0) != corner)
                        return false;
                    corner = edge.Point(1);
                }
            }

        return true;
    }

    /// Computes the shape's bounding box.
    /// double[left, bottom, right, top]
    public void Bounds(double[] box)
    {
        foreach (var contour in this)
            contour.Bounds(box);
    }
}