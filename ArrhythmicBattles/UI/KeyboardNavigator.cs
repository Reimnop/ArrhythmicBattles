using System.Drawing;
using ArrhythmicBattles.Util;
using FlexFramework.Core.Data;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;
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

    private readonly IInputProvider inputProvider;
    
    private readonly RectEntity rectEntity= new RectEntity()
    {
        Radius = 8.0f
    };

    private SimpleAnimator<Bounds> highlightAnimator = null!;

    private NavNode currentNode;
    private Bounds currentHighlightRect;

    private Vector2 currentRectSize;

    public KeyboardNavigator(IInputProvider inputProvider, NavNode rootNode)
    {
        this.inputProvider = inputProvider;
        RootNode = rootNode;
        
        RootNode.Element.IsFocused = true;
        currentNode = RootNode;
    }

    public override void Start()
    {
        highlightAnimator = new SimpleAnimator<Bounds>(
            (left, right, factor) => new Bounds(
                MathHelper.Lerp(left.X0, right.X0, factor),
                MathHelper.Lerp(left.Y0, right.Y0, factor),
                MathHelper.Lerp(left.X1, right.X1, factor),
                MathHelper.Lerp(left.Y1, right.Y1, factor)),
            value => currentHighlightRect = value,
            RootNode.Element.GetBounds(),
            10.0f);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        highlightAnimator.Update(args);

        if (inputProvider.GetKeyDown(Keys.Up))
        {
            AdvanceTo(currentNode.Top);
        }
        
        if (inputProvider.GetKeyDown(Keys.Down))
        {
            AdvanceTo(currentNode.Bottom);
        }
        
        if (inputProvider.GetKeyDown(Keys.Left))
        {
            AdvanceTo(currentNode.Left);
        }
        
        if (inputProvider.GetKeyDown(Keys.Right))
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

        rectEntity.Min = Vector2.Zero;
        rectEntity.Max = actualRectSize;
    }
    
    public void Render(RenderArgs args)
    {
        RegenRectIfNecessary();
        
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(currentHighlightRect.X0, currentHighlightRect.Y0, 0.0f);
        rectEntity.Render(args);
        matrixStack.Pop();
    }

    public void Dispose()
    {
        rectEntity.Dispose();
    }
}