using OpenTK.Mathematics;

namespace ArrhythmicBattles.Util;

public static class MeshGenerator
{
    public static Vector2[] GenerateBorder(Vector2 min, Vector2 max, float thickness)
    {
        List<Vector2> vertices = new List<Vector2>();
        vertices.AddRange(GenerateRectangle(min, new Vector2(min.X + thickness, max.Y)));
        vertices.AddRange(GenerateRectangle(new Vector2(max.X - thickness, min.Y), max));
        vertices.AddRange(GenerateRectangle(new Vector2(min.X + thickness, min.Y), new Vector2(max.X - thickness, min.Y + thickness)));
        vertices.AddRange(GenerateRectangle(new Vector2(min.X + thickness, max.Y - thickness), new Vector2(max.X - thickness, max.Y)));
        return vertices.ToArray();
    }
    
    public static Vector2[] GenerateRoundedRectangle(Vector2 min, Vector2 max, float radius, int resolution = 8)
    {
        if (radius == 0.0)
        {
            return GenerateRectangle(min, max);
        }
        
        Vector2 a = new Vector2(max.X - radius, max.Y - radius);
        Vector2 b = new Vector2(min.X + radius, max.Y - radius);
        Vector2 c = new Vector2(min.X + radius, min.Y + radius);
        Vector2 d = new Vector2(max.X - radius, min.Y + radius);

        List<Vector2> vertices = new List<Vector2>();
        vertices.AddRange(GenerateCircleArch(resolution, 0.0f, MathF.PI * 0.5f)
            .Select(value => value * radius + a));
        vertices.AddRange(GenerateCircleArch(resolution, MathF.PI * 0.5f, MathF.PI)
            .Select(value => value * radius + b));
        vertices.AddRange(GenerateCircleArch(resolution, MathF.PI, MathF.PI * 1.5f)
            .Select(value => value * radius + c));
        vertices.AddRange(GenerateCircleArch(resolution, MathF.PI * 1.5f, MathF.PI * 2.0f)
            .Select(value => value * radius + d));
        vertices.AddRange(GenerateRectangle(new Vector2(min.X, min.Y + radius), b));
        vertices.AddRange(GenerateRectangle(d, new Vector2(max.X, max.Y - radius)));
        vertices.AddRange(GenerateRectangle(new Vector2(min.X + radius, min.Y), new Vector2(max.X - radius, max.Y)));
        return vertices.ToArray();
    }

    public static Vector2[] GenerateRectangle(Vector2 min, Vector2 max)
    {
        Vector2 lengths = max - min;
        if (lengths.X * lengths.Y == 0.0)
        {
            return Array.Empty<Vector2>();
        }
        
        return new Vector2[] 
        {
            new Vector2(max.X, max.Y),
            new Vector2(min.X, max.Y),
            new Vector2(min.X, min.Y),
            new Vector2(max.X, max.Y),
            new Vector2(min.X, min.Y),
            new Vector2(max.X, min.Y)
        };
    }
    
    public static Vector2[] GenerateCircleArch(int resolution, float startAngle = 0.0f, float endAngle = MathF.PI * 2.0f)
    {
        List<Vector2> vertices = new List<Vector2>();
        
        for (int i = 0; i < resolution; i++)
        {
            float alpha = MathHelper.Lerp(startAngle, endAngle, i / (float) resolution);
            float beta = MathHelper.Lerp(startAngle, endAngle, (i + 1) / (float) resolution);
            Vector2 a = new Vector2(MathF.Cos(alpha), MathF.Sin(alpha));
            Vector2 b = new Vector2(MathF.Cos(beta), MathF.Sin(beta));
            vertices.Add(Vector2.Zero);
            vertices.Add(a);
            vertices.Add(b);
        }

        return vertices.ToArray();
    }
}