using System.Drawing;
using ArrhythmicBattles.Util;
using FlexFramework.Core.Data;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.UI;

public class NavNode
{
    public NavNode? Top { get; set; }
    public NavNode? Bottom { get; set; }
    public NavNode? Left { get; set; }
    public NavNode? Right { get; set; }

    public UIElement Element { get; set; }

    public NavNode(UIElement element)
    {
        Element = element;
    }
    
    public NavNode AppendTop(UIElement element)
    {
        Top = new NavNode(element);
        Top.Bottom = this;
        return Top;
    }

    public NavNode AppendBottom(UIElement element)
    {
        Bottom = new NavNode(element);
        Bottom.Top = this;
        return Bottom;
    }
    
    public NavNode AppendLeft(UIElement element)
    {
        Left = new NavNode(element);
        Left.Right = this;
        return Left;
    }
    
    public NavNode AppendRight(UIElement element)
    {
        Right = new NavNode(element);
        Right.Left = this;
        return Right;
    }
}

public delegate void NodeSelectedEventHandler(NavNode node);

public class KeyboardNavigator : Entity, IRenderable, IDisposable
{
    public NavNode RootNode { get; }
    public event NodeSelectedEventHandler? OnNodeSelected;
    
    private readonly InputSystem input;
    private readonly InputCapture capture;
    private readonly MeshEntity meshEntity;
    private readonly Mesh<Vertex> mesh;
    
    private SimpleAnimator<RectangleF> highlightAnimator = null!;

    private NavNode currentNode;
    private RectangleF currentHighlightRect;

    private Vector2 currentRectSize;

    public KeyboardNavigator(InputInfo inputInfo, NavNode rootNode)
    {
        input = inputInfo.InputSystem;
        capture = inputInfo.InputCapture;
        RootNode = rootNode;
        
        RootNode.Element.IsFocused = true;
        currentNode = RootNode;

        mesh = new Mesh<Vertex>("mesh");

        meshEntity = new MeshEntity();
        meshEntity.Mesh = mesh;
    }

    public override void Start()
    {
        highlightAnimator = new SimpleAnimator<RectangleF>(
            (left, right, factor) =>
            {
                float t = Easing.QuadInOut(factor);

                return new RectangleF(
                    MathHelper.Lerp(left.X, right.X, t),
                    MathHelper.Lerp(left.Y, right.Y, t),
                    MathHelper.Lerp(left.Width, right.Width, t),
                    MathHelper.Lerp(left.Height, right.Height, t));
            },
            value => currentHighlightRect = value,
            RootNode.Element.GetBounds(),
            10.0f);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        highlightAnimator.Update(args);
        meshEntity.Update(args);
        
        if (input.GetKeyDown(capture, Keys.Up))
        {
            AdvanceTo(currentNode.Top);
        }
        
        if (input.GetKeyDown(capture, Keys.Down))
        {
            AdvanceTo(currentNode.Bottom);
        }
        
        if (input.GetKeyDown(capture, Keys.Left))
        {
            AdvanceTo(currentNode.Left);
        }
        
        if (input.GetKeyDown(capture, Keys.Right))
        {
            AdvanceTo(currentNode.Right);
        }
    }
    
    private void AdvanceTo(NavNode? node)
    {
        if (node == null)
        {
            return;
        }
        
        currentNode.Element.IsFocused = false;
        node.Element.IsFocused = true;
        currentNode = node;
        
        OnNodeSelected?.Invoke(node);
        
        highlightAnimator.LerpTo(node.Element.GetBounds());
    }

    private void RegenRectIfNecessary()
    {
        Vector2 actualRectSize = new Vector2(currentHighlightRect.Width, currentHighlightRect.Height);
        if (currentRectSize == actualRectSize)
        {
            return;
        }

        currentRectSize = actualRectSize;
        
        Vertex[] vertices = MeshGenerator.GenerateRoundedRectangle(Vector2.Zero, new Vector2(actualRectSize.X, actualRectSize.Y), 8)
            .Select(pos => new Vertex(pos.X, pos.Y, 0.0f, actualRectSize.X / pos.X, actualRectSize.Y / pos.Y))
            .ToArray();
        mesh.LoadData(vertices);
    }
    
    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        RegenRectIfNecessary();
        
        matrixStack.Push();
        matrixStack.Translate(currentHighlightRect.X, currentHighlightRect.Y, 0.0f);
        meshEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        mesh.Dispose();
    }
}