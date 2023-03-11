using MsdfGenNet;

using Shape shape = new Shape();
using Contour contour = new Contour();

contour.AddEdge(new Edge(new Vector2d(-1.0, -1.0), new Vector2d(1.0, 1.0), EdgeColor.White));
contour.AddEdge(new Edge(new Vector2d(1.0, 1.0), new Vector2d(1.0, -1.0), EdgeColor.White));
contour.AddEdge(new Edge(new Vector2d(1.0, -1.0), new Vector2d(-1.0, -1.0), EdgeColor.White));

Vector2d scale = new Vector2d(1.0, 1.0);
Vector2d translate = new Vector2d(0.0, 0.0);
using Projection projection = new Projection(ref scale, ref translate);

GeneratorConfig config = new GeneratorConfig();

Bitmap<float> bitmap = new Bitmap<float>(256, 256, 1);
MsdfGen.GenerateSDF(bitmap, shape, projection, 4.0, ref config);