using System.Diagnostics;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class VisualElement : Element, IRenderable
{
#if DEBUG_SHOW_BOUNDING_BOXES // This will blow up if you have multiple OpenGL contexts, but it's only for debugging anyway
    private static readonly Mesh<Vertex> DebugMesh = new Mesh<Vertex>("debug");

    static VisualElement()
    {
        Vertex[] debugVertices =
        {
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, -0.5f, 0.0f, 1.0f, 0.0f)
        };
        
        DebugMesh.LoadData(debugVertices);
    }
#endif

    public Transform RenderTransform { get; set; } = new Transform();
    
    protected Bounds BoundingBox { get; private set; }
    protected Bounds ElementBounds { get; private set; }
    protected Bounds ContentBounds { get; private set; }

    protected VisualElement(params Element[] elements) : base(elements)
    {
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        CalculateBounds(constraintBounds, out Bounds boundingBox, out Bounds elementBounds, out Bounds contentBounds);
        BoundingBox = boundingBox;
        ElementBounds = elementBounds;
        ContentBounds = contentBounds;
    }

    public abstract void Render(RenderArgs args);

    [Conditional("DEBUG_SHOW_BOUNDING_BOXES")]
    protected void DrawDebugBoxes(RenderArgs args)
    {
#if DEBUG_SHOW_BOUNDING_BOXES
        RenderBounds(BoundingBox, Color4.Red, args);
        RenderBounds(ElementBounds, Color4.Green, args);
        RenderBounds(ContentBounds, Color4.Blue, args);
#endif
    }

    [Conditional("DEBUG_SHOW_BOUNDING_BOXES")]
    private void RenderBounds(Bounds bounds, Color4 color, RenderArgs args)
    {
#if DEBUG_SHOW_BOUNDING_BOXES
        MatrixStack matrixStack = args.MatrixStack;
        Vector2 size = bounds.Max - bounds.Min;
        
        matrixStack.Push();
        matrixStack.Translate(0.5f, 0.5f, 0.0f);
        matrixStack.Scale(size.X, size.Y, 1.0f);
        matrixStack.Translate(bounds.X0, bounds.Y0, 0.0f);
        VertexDrawData vertexDrawData = new VertexDrawData(DebugMesh.VertexArray, DebugMesh.Count, matrixStack.GlobalTransformation * args.CameraData.View * args.CameraData.Projection, null, color, PrimitiveType.LineLoop);
        args.Renderer.EnqueueDrawData(args.LayerId, vertexDrawData);
        matrixStack.Pop();
#endif
    }
}