using MsdfGenNet;

using Shape shape = new Shape();
using Contour contour = new Contour();

contour.AddEdge(new Edge(new Point2(-1.0, -1.0), new Point2(1.0, 1.0), EdgeColor.Black));
contour.AddEdge(new Edge(new Point2(1.0, 1.0), new Point2(1.0, -1.0), EdgeColor.Black));
contour.AddEdge(new Edge(new Point2(1.0, -1.0), new Point2(-1.0, -1.0), EdgeColor.Black));

shape.AddContour(contour);

Console.WriteLine(shape.EdgeCount());