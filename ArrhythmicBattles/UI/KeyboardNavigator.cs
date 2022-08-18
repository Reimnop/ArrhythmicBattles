using System.Drawing;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.Data;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
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

public class KeyboardNavigator : Entity, IRenderable
{
    public NavNode RootNode { get; }
    
    private readonly FlexFrameworkMain engine;
    private readonly MeshEntity meshEntity;
    private readonly Mesh<Vertex> mesh;
    private readonly SimpleAnimator<Rectangle> highlightAnimator;

    private NavNode currentNode;
    private Rectangle currentHighlightRect;

    private Vector2i currentRectSize;

    public KeyboardNavigator(FlexFrameworkMain engine, NavNode rootNode)
    {
        this.engine = engine;
        RootNode = rootNode;
        rootNode.Element.IsFocused = true;
        currentNode = rootNode;

        mesh = new Mesh<Vertex>("mesh");
        mesh.Attribute(3, 0, VertexAttribType.Float, false);
        mesh.Attribute(2, 3 * sizeof(float), VertexAttribType.Float, false);

        meshEntity = new MeshEntity();
        meshEntity.Mesh = mesh;

        highlightAnimator = new SimpleAnimator<Rectangle>(
            (left, right, factor) =>
            {
                double t = Easing.InOutCirc(factor);

                return new Rectangle(
                    (int) MathHelper.Lerp(left.X, right.X, t),
                    (int) MathHelper.Lerp(left.Y, right.Y, t),
                    (int) MathHelper.Lerp(left.Width, right.Width, t),
                    (int) MathHelper.Lerp(left.Height, right.Height, t));
            },
            value => currentHighlightRect = value,
            () => rootNode.Element.GetBounds(),
            10.0);
    }

    public override void Update(UpdateArgs args)
    {
        highlightAnimator.Update(args.DeltaTime);
        meshEntity.Update(args);
        
        if (engine.Input.GetKeyDown(Keys.Up))
        {
            AdvanceTo(currentNode.Top);
        }
        
        if (engine.Input.GetKeyDown(Keys.Down))
        {
            AdvanceTo(currentNode.Bottom);
        }
        
        if (engine.Input.GetKeyDown(Keys.Left))
        {
            AdvanceTo(currentNode.Left);
        }
        
        if (engine.Input.GetKeyDown(Keys.Right))
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
        
        highlightAnimator.LerpTo(() => node.Element.GetBounds());
    }

    private void RegenRectIfNecessary()
    {
        Vector2i actualRectSize = new Vector2i(currentHighlightRect.Width, currentHighlightRect.Height);
        if (currentRectSize == actualRectSize)
        {
            return;
        }

        currentRectSize = actualRectSize;
        
        Vertex[] vertices = MeshGenerator.GenerateRoundedRectangle(new Vector2d(actualRectSize.X, actualRectSize.Y), 8)
            .Select(pos => new Vertex((float) pos.X, (float) pos.Y, 0.0f, actualRectSize.X / (float) pos.X, actualRectSize.Y / (float) pos.Y))
            .ToArray();
        mesh.LoadData(vertices);
    }
    
    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        RegenRectIfNecessary();
        
        matrixStack.Push();
        matrixStack.Translate(currentHighlightRect.X, currentHighlightRect.Y, 0.0);
        meshEntity.Render(renderer, layerId, matrixStack, cameraData);
        matrixStack.Pop();
    }

    public override void Dispose()
    {
        meshEntity.Dispose();
        mesh.Dispose();
    }
}