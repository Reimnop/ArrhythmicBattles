using System.Drawing;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core.UserInterface.Elements;

public class ButtonElement : VisualElement, IUpdateable, IDisposable
{
    public event Action? Click;
    
    private readonly Interactivity interactivity;
    
    private readonly Mesh<Vertex> mesh;
    private readonly List<Vector2> vertexPositions = new List<Vector2>();
    
    private bool hovered;

    public ButtonElement(IInputProvider inputProvider, params Element[] children) : base(children)
    {
        interactivity = new Interactivity(inputProvider);
        interactivity.MouseButtonDown += OnMouseButtonDown;
        interactivity.MouseEnter += OnMouseEnter;
        interactivity.MouseLeave += OnMouseLeave;
    }

    private void OnMouseLeave()
    {
        hovered = false;
    }

    private void OnMouseEnter()
    {
        hovered = true;
    }

    private void OnMouseButtonDown(MouseButton button)
    {
        if (button == MouseButton.Left)
        {
            Click?.Invoke();
        }
    }

    public void Update(UpdateArgs args)
    {
        interactivity.Update();
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
        
        interactivity.Bounds = ElementBounds;
        
        vertexPositions.Clear();
        MeshGenerator.GenerateRoundedRectangle(vertexPositions, ElementBounds.Min, ElementBounds.Max, 4.0f);
        
        Span<Vertex> vertices = stackalloc Vertex[vertexPositions.Count];
        for (int i = 0; i < vertexPositions.Count; i++)
        {
            Vector2 pos = vertexPositions[i];
            Vector2 relativePos = pos - ElementBounds.Min;
            Vector2 uv = new Vector2(relativePos.X / ElementBounds.Width, relativePos.Y / ElementBounds.Height);
            vertices[i] = new Vertex(new Vector3(pos), uv);
        }
        
        mesh.LoadData(vertices);
    }

    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        CameraData cameraData = args.CameraData;
        int layerId = args.LayerId;
        
        matrixStack.Push();
        VertexDrawData vertexDrawData = new VertexDrawData(mesh.VertexArray, mesh.Count, matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection, null, hovered ? new Color4(0.7f, 0.7f, 0.7f, 1.0f) : new Color4(0.9f, 0.9f, 0.9f, 1.0f), PrimitiveType.Triangles);
        args.Renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
        
        DrawDebugBoxes(args);
    }

    public void Dispose()
    {
        mesh.Dispose();
    }
}