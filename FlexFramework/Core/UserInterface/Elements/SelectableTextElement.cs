using System.Dynamic;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Text;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TextCopy;

namespace FlexFramework.Core.UserInterface.Elements;

public class SelectableTextElement : VisualElement, IUpdateable, IRenderable
{
    public string Text
    {
        get => textEntity.Text;
        set
        {
            textEntity.Text = value;
            selectionText = TextShaper.GetSelectionText(
                font, 
                value, 
                textEntity.HorizontalAlignment,
                textEntity.VerticalAlignment);

            if (autoHeight)
            {
                var lines = value.Split('\n').Length;
                var height = lines * (font.Metrics.Height >> 6) * textEntity.EmSize;
                Height = height;
            }
        }
    }

    public Color4 Color
    {
        get => textEntity.Color;
        set => textEntity.Color = value;
    }

    private readonly TextEntity textEntity;
    private readonly ScopedInputProvider inputProvider;
    private readonly Font font;
    private readonly bool autoHeight;
    private SelectionText? selectionText;
    
    private (int, int) selection = (-1, -1);
    private ScopedInputProvider? dragInputProvider;

    public SelectableTextElement(Font font, ScopedInputProvider inputProvider, bool autoHeight = true, params Element[] children) : base(children)
    {
        this.font = font;
        this.inputProvider = inputProvider;
        this.autoHeight = autoHeight;

        textEntity = new TextEntity(font);
        textEntity.BaselineOffset = font.Metrics.Height;
    }
    
    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
    }
    
    public void Update(UpdateArgs args)
    {
        if (selectionText == null)
            return;

        if (inputProvider.GetMouseDown(MouseButton.Left)) // Start drag
        {
            var hoveredCharacter = GetHoveredCharacter(selectionText, inputProvider.MousePosition - ElementBounds.Min, textEntity.BaselineOffset) ?? -1;
            selection = (hoveredCharacter, hoveredCharacter);

            if (hoveredCharacter != -1)
                dragInputProvider = inputProvider.InputSystem.AcquireInputProvider();
        }
        
        if (dragInputProvider != null && dragInputProvider.GetMouseUp(MouseButton.Left)) // End drag
        {
            dragInputProvider.Dispose();
            dragInputProvider = null;
        }
        
        if (dragInputProvider != null) // Drag
        {
            var dragCharacter = GetDragCharacter(selectionText, dragInputProvider.MousePosition - ElementBounds.Min, textEntity.BaselineOffset);
            selection = (selection.Item1, dragCharacter);
        }
        
        if (inputProvider.GetKey(Keys.LeftControl) && inputProvider.GetKeyDown(Keys.C) && selection.Item1 != -1 && selection.Item2 != -1) // Copy
        {
            var selectionStart = Math.Min(selection.Item1, selection.Item2);
            var selectionEnd = Math.Max(selection.Item1, selection.Item2);
            
            var text = Text.Substring(selectionStart, selectionEnd - selectionStart + 1);
            ClipboardService.SetText(text);
        }
    }

    public override void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;

        matrixStack.Push();
        matrixStack.Translate(ElementBounds.X0, ElementBounds.Y0, 0.0f);
        textEntity.Render(args);

        // Render selection boxes
        // TODO: Optimize this
        if (selectionText != null)
        {
            foreach (var (bounds, index) in EnumerateSelectionBoxes(selectionText, textEntity.BaselineOffset))
            {
                var selectionStart = Math.Min(selection.Item1, selection.Item2);
                var selectionEnd = Math.Max(selection.Item1, selection.Item2);
                
                if (index < selectionStart || index > selectionEnd)
                    continue; // Skip

                var pos = bounds.Min;
                var size = bounds.Size;
                
                matrixStack.Push();
                matrixStack.Translate(0.5f, 0.5f, 0.0f);
                matrixStack.Scale(size.X, size.Y, 1.0f);
                matrixStack.Translate(pos.X, pos.Y, 0.0f);

                var transform = matrixStack.GlobalTransformation * args.CameraData.View * args.CameraData.Projection;
                var vertexDrawData = new VertexDrawData(
                    DefaultAssets.QuadMesh.ReadOnly, 
                    transform, 
                    null, 
                    new Color4(100, 149, 237, 127), 
                    PrimitiveType.Triangles);
                args.CommandList.AddDrawData(args.LayerType, vertexDrawData);
                
                matrixStack.Pop();
            }
        }

        matrixStack.Pop();
    }
    
    private static int? GetHoveredCharacter(SelectionText selectionText, Vector2 mousePosition, int baselineOffset)
    {
        if (!IsInBounds(selectionText, mousePosition, baselineOffset)) 
            return null;
        
        foreach (var (bounds, index) in EnumerateSelectionBoxes(selectionText, baselineOffset))
        {
            if (bounds.ContainsInclusive(mousePosition))
            {
                return index;
            }
        }
        
        return null;
    }

    private static int GetDragCharacter(SelectionText selectionText, Vector2 mousePosition, int baselineOffset)
    {
        // Get the line that the mouse is on
        var line = GetDragLine(selectionText, mousePosition, baselineOffset);
        
        // Get the character that the mouse is on
        
        // If the mouse is to the left of the first character, return the first character
        if (mousePosition.X < line.SelectablePositions[0] / 64.0f)
            return line.SelectableIndices[0];
        
        // If the mouse is to the right of the last character, return the last character
        if (mousePosition.X > line.SelectablePositions[^1] / 64.0f)
            return line.SelectableIndices[^1];
        
        // Otherwise, find the character that the mouse is on
        for (var i = 0; i < line.SelectablePositions.Count - 1; i++)
        {
            var left = line.SelectablePositions[i] / 64.0f;
            var right = line.SelectablePositions[i + 1] / 64.0f;
            
            if (left <= mousePosition.X && mousePosition.X <= right)
            {
                var leftDistance = mousePosition.X - left;
                var rightDistance = right - mousePosition.X;
                
                if (leftDistance < rightDistance)
                    return line.SelectableIndices[i];
                else
                    return line.SelectableIndices[i + 1];
            }
        }
        
        throw new Exception("THE CODE BLEW UP, THIS SHOULD NEVER HAPPEN, WTF IS GOING ON");
    }
    
    private static SelectionLine GetDragLine(SelectionText selectionText, Vector2 mousePosition, int baselineOffset)
    {
        // If mouse is above the first line, return the first line
        if (mousePosition.Y < selectionText.Lines[0].Top / 64.0f + baselineOffset / 64.0f)
            return selectionText.Lines[0];
        
        // If mouse is below the last line, return the last line
        if (mousePosition.Y > selectionText.Lines[^1].Bottom / 64.0f + baselineOffset / 64.0f)
            return selectionText.Lines[^1];
        
        // Otherwise, find the line that the mouse is on
        foreach (var line in selectionText.Lines)
        {
            if (line.Top / 64.0f + baselineOffset / 64.0f <= mousePosition.Y && mousePosition.Y <= line.Bottom / 64.0f + baselineOffset / 64.0f)
                return line;
        }
        
        throw new Exception("THE CODE BLEW UP, THIS SHOULD NEVER HAPPEN, WTF IS GOING ON");
    }

    private static bool IsInBounds(SelectionText selectionText, Vector2 mousePosition, int baselineOffset)
    {
        var x0 = selectionText.MinX / 64.0f;
        var y0 = selectionText.MinY / 64.0f + baselineOffset / 64.0f;
        var x1 = selectionText.MaxX / 64.0f;
        var y1 = selectionText.MaxY / 64.0f + baselineOffset / 64.0f;
        var box = GetBox(x0, y0, x1, y1);
        
        return box.ContainsInclusive(mousePosition);
    }

    private static IEnumerable<(Box2, int)> EnumerateSelectionBoxes(SelectionText selectionText, int baselineOffset)
    {
        foreach (var line in selectionText.Lines)
        {
            for (int i = 0; i < line.SelectablePositions.Count - 1; i++)
            {
                var x0 = line.SelectablePositions[i] / 64.0f;
                var y0 = line.Bottom / 64.0f + baselineOffset / 64.0f;
                var x1 = line.SelectablePositions[i + 1] / 64.0f;
                var y1 = line.Top / 64.0f + baselineOffset / 64.0f;

                yield return (GetBox(x0, y0, x1, y1), line.SelectableIndices[i + 1]);
            }
        }
    }
    
    private static Box2 GetBox(float x0, float y0, float x1, float y1)
    {
        var min = new Vector2(Math.Min(x0, x1), Math.Min(y0, y1));
        var max = new Vector2(Math.Max(x0, x1), Math.Max(y0, y1));
        return new Box2(min, max);
    }
}