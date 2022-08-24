using OpenTK.Mathematics;

namespace ArrhythmicBattles.Util;

public static class MeshGenerator
{
    public static Vector2d[] GenerateBorder(Vector2d min, Vector2d max, double thickness)
    {
        List<Vector2d> vertices = new List<Vector2d>();
        vertices.AddRange(GenerateRectangle(min, new Vector2d(min.X + thickness, max.Y)));
        vertices.AddRange(GenerateRectangle(new Vector2d(max.X - thickness, min.Y), max));
        vertices.AddRange(GenerateRectangle(new Vector2d(min.X + thickness, min.Y), new Vector2d(max.X - thickness, min.Y + thickness)));
        vertices.AddRange(GenerateRectangle(new Vector2d(min.X + thickness, max.Y - thickness), new Vector2d(max.X - thickness, max.Y)));
        return vertices.ToArray();
    }
    
    public static Vector2d[] GenerateRoundedRectangle(Vector2d min, Vector2d max, double radius, int resolution = 8)
    {
        if (radius == 0.0)
        {
            return GenerateRectangle(min, max);
        }
        
        Vector2d a = new Vector2d(max.X - radius, max.Y - radius);
        Vector2d b = new Vector2d(min.X + radius, max.Y - radius);
        Vector2d c = new Vector2d(min.X + radius, min.Y + radius);
        Vector2d d = new Vector2d(max.X - radius, min.Y + radius);

        List<Vector2d> vertices = new List<Vector2d>();
        vertices.AddRange(GenerateCircleArch(resolution, 0.0, Math.PI * 0.5)
            .Select(value => value * radius + a));
        vertices.AddRange(GenerateCircleArch(resolution, Math.PI * 0.5, Math.PI)
            .Select(value => value * radius + b));
        vertices.AddRange(GenerateCircleArch(resolution, Math.PI, Math.PI * 1.5)
            .Select(value => value * radius + c));
        vertices.AddRange(GenerateCircleArch(resolution, Math.PI * 1.5, Math.PI * 2.0)
            .Select(value => value * radius + d));
        vertices.AddRange(GenerateRectangle(new Vector2d(min.X, min.Y + radius), b));
        vertices.AddRange(GenerateRectangle(d, new Vector2d(max.X, max.Y - radius)));
        vertices.AddRange(GenerateRectangle(new Vector2d(min.X + radius, min.Y), new Vector2d(max.X - radius, max.Y)));
        return vertices.ToArray();
    }

    public static Vector2d[] GenerateRectangle(Vector2d min, Vector2d max)
    {
        Vector2d lengths = max - min;
        if (lengths.X * lengths.Y == 0.0)
        {
            return Array.Empty<Vector2d>();
        }
        
        return new Vector2d[] 
        {
            new Vector2d(max.X, max.Y),
            new Vector2d(min.X, max.Y),
            new Vector2d(min.X, min.Y),
            new Vector2d(max.X, max.Y),
            new Vector2d(min.X, min.Y),
            new Vector2d(max.X, min.Y)
        };
    }
    
    public static Vector2d[] GenerateCircleArch(int resolution, double startAngle = 0.0, double endAngle = Math.PI * 2.0)
    {
        List<Vector2d> vertices = new List<Vector2d>();
        
        for (int i = 0; i < resolution; i++)
        {
            double alpha = MathHelper.Lerp(startAngle, endAngle, i / (double) resolution);
            double beta = MathHelper.Lerp(startAngle, endAngle, (i + 1) / (double) resolution);
            Vector2d a = new Vector2d(Math.Cos(alpha), Math.Sin(alpha));
            Vector2d b = new Vector2d(Math.Cos(beta), Math.Sin(beta));
            vertices.Add(Vector2d.Zero);
            vertices.Add(a);
            vertices.Add(b);
        }

        return vertices.ToArray();
    }
}