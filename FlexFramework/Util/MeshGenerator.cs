using OpenTK.Mathematics;
using Poly2Tri;
using Poly2Tri.Triangulation.Polygon;
using Poly2Tri.Utility;

namespace FlexFramework.Util;

public delegate void VertexConsumer(Vector2 vertex);

public static class MeshGenerator
{
    public static int GenerateRoundedRectangle(VertexConsumer vertexConsumer, Box2 bounds, float radius, float borderThickness = float.PositiveInfinity, int resolution = 8)
    {
        var polygon = GenerateRectanglePoly(bounds, radius, resolution);
        
        // Generate inner path if border thickness is less than half of the smallest dimension
        var maxThickness = MathF.Min(bounds.HalfSize.X, bounds.HalfSize.Y);
        if (borderThickness < maxThickness)
        {
            var innerBounds = new Box2(bounds.Min + new Vector2(borderThickness), bounds.Max - new Vector2(borderThickness));
            polygon.AddHole(GenerateRectanglePoly(innerBounds, radius - borderThickness, resolution));
        }
        
        // Triangulate
        P2T.Triangulate(polygon);
        
        // Generate vertices
        foreach (var triangle in polygon.Triangles)
        {
            vertexConsumer(Point2DToVector2(triangle.Points[0]));
            vertexConsumer(Point2DToVector2(triangle.Points[1]));
            vertexConsumer(Point2DToVector2(triangle.Points[2]));
        }
        
        return polygon.Triangles.Count * 3;
    }
    
    private static Vector2 Point2DToVector2(Point2D point)
    {
        return new Vector2((float) point.X, (float) point.Y);
    }

    private static Polygon GenerateRectanglePoly(Box2 bounds, float radius, int resolution)
    {
        var min = bounds.Min;
        var max = bounds.Max;
        
        var result = new Polygon();
        AppendCircleArch(result, new Vector2(max.X - radius, min.Y + radius), radius, resolution, MathF.PI * 0.0f, MathF.PI * 0.5f);
        result.Add(new Point2D(max.X - radius, min.Y));
        AppendCircleArch(result, new Vector2(min.X + radius, min.Y + radius), radius, resolution, MathF.PI * 0.5f, MathF.PI * 1.0f);
        result.Add(new Point2D(min.X, min.Y + radius));
        AppendCircleArch(result, new Vector2(min.X + radius, max.Y - radius), radius, resolution, MathF.PI * 1.0f, MathF.PI * 1.5f);
        result.Add(new Point2D(min.X + radius, max.Y));
        AppendCircleArch(result, new Vector2(max.X - radius, max.Y - radius), radius, resolution, MathF.PI * 1.5f, MathF.PI * 2.0f);
        result.Add(new Point2D(max.X, max.Y - radius));
        return result;
    }

    private static void AppendCircleArch(Polygon polygon, Vector2 center, float radius, int resolution, float startAngle, float endAngle)
    {
        if (radius == 0.0f)
            return;
        
        for (int i = 0; i < resolution; i++)
        {
            var theta = MathHelper.Lerp(startAngle, endAngle, i / (float) resolution);
            var point = new Point2D(MathF.Cos(theta) * radius + center.X, MathF.Sin(theta) * radius + center.Y);
            polygon.Add(point);
        }
    }

    public static void GenerateRectangle(IList<Vector2> vertices, Box2 bounds)
    {
        var max = bounds.Max;
        var min = bounds.Min;
        var lengths = max - min;
        if (lengths.X * lengths.Y == 0.0)
            return;

        vertices.Add(new Vector2(max.X, max.Y));
        vertices.Add(new Vector2(min.X, max.Y));
        vertices.Add(new Vector2(min.X, min.Y));
        vertices.Add(new Vector2(max.X, max.Y));
        vertices.Add(new Vector2(min.X, min.Y));
        vertices.Add(new Vector2(max.X, min.Y));
    }
    
    public static void GenerateCircleArch(IList<Vector2> vertices, Vector2 center, float radius, int resolution = 8, float startAngle = 0.0f, float endAngle = MathF.PI * 2.0f)
    {
        for (int i = 0; i < resolution; i++)
        {
            var alpha = MathHelper.Lerp(startAngle, endAngle, i / (float) resolution);
            var beta = MathHelper.Lerp(startAngle, endAngle, (i + 1) / (float) resolution);
            var a = new Vector2(MathF.Cos(alpha), MathF.Sin(alpha));
            var b = new Vector2(MathF.Cos(beta), MathF.Sin(beta));
            vertices.Add(center);
            vertices.Add(a * radius + center);
            vertices.Add(b * radius + center);
        }
    }
}