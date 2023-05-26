using FlexFramework.Core.Data;
using FlexFramework.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core.Entities;

public class SelectableTextEntity : Entity, IRenderable
{
    private static readonly Mesh<Vertex> QuadMesh = new("debug");

    static SelectableTextEntity()
    {
        Vertex[] vertices =
        {
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, -0.5f, 0.0f, 1.0f, 0.0f)
        };
        
        QuadMesh.SetData(vertices, ReadOnlySpan<int>.Empty);
    }
    
    public string Text
    {
        get => textEntity.Text;
        set
        {
            textEntity.Text = value;
            UpdateTextData();
        }
    }

    public Font Font
    {
        get => textEntity.Font;
        set
        {
            textEntity.Font = value;
            UpdateTextData();
        }
    }
    
    public int BaselineOffset
    {
        get => textEntity.BaselineOffset;
        set => textEntity.BaselineOffset = value;
    }

    public float EmSize
    {
        get => textEntity.EmSize;
        set => textEntity.EmSize = value;
    }
    
    public Color4 Color
    {
        get => textEntity.Color;
        set => textEntity.Color = value;
    }
    
    public HorizontalAlignment HorizontalAlignment
    {
        get => textEntity.HorizontalAlignment;
        set
        {
            textEntity.HorizontalAlignment = value;
            UpdateTextData();
        }
    }

    public VerticalAlignment VerticalAlignment
    {
        get => textEntity.VerticalAlignment;
        set
        {
            textEntity.VerticalAlignment = value;
            UpdateTextData();
        }
    }

    private TextEntity textEntity;
    private MeshEntity meshEntity;
    
    public Vector2 Position { get; set; }
    
    private SelectionText? selectionText;
    private readonly IInputProvider inputProvider;
    
    private bool dragging = false;
    private (int, int) selection = (0, 0);

    public SelectableTextEntity(Font font, IInputProvider inputProvider)
    {
        this.inputProvider = inputProvider;
        textEntity = new TextEntity(font);
        meshEntity = new MeshEntity(QuadMesh);
    }
    
    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        textEntity.Update(args);

        if (dragging && inputProvider.GetMouseUp(MouseButton.Left))
            dragging = false;

        if (selectionText == null)
            return; // No selection text to check
        
        if (!inputProvider.InputAvailable)
            return; // Input is not available
        
        // Check if mouse is hovering over any line
        var mousePos = inputProvider.MousePosition;
        var line = CheckForLineCollision(selectionText, mousePos);
        if (line == null)
            return; // No line is being hovered over

        if (dragging)
        {
            int? character = CheckForCharacterCollision(line, mousePos); // Check if mouse is hovering over any character
            if (character != null)
                selection.Item2 = character.Value;
        }

        if (inputProvider.GetMouseDown(MouseButton.Left))
        {
            int? character = CheckForCharacterCollision(line, mousePos); // Check if mouse is hovering over any character
            if (character == null)
                return; // No character is being hovered over
            selection = (character.Value, character.Value);
            dragging = true;
        }
    }

    private SelectionLine? CheckForLineCollision(SelectionText selectionText, Vector2 mousePos)
    {
        // Linear search
        // TODO: Turn this into a binary search
        
        foreach (var line in selectionText.Lines)
        {
            var min = new Vector2(line.SelectablePositions[0] / 64.0f, line.Top / 64.0f) * EmSize + Position + new Vector2(0.0f, 4.0f);
            var max = new Vector2(line.SelectablePositions[^1] / 64.0f, line.Bottom / 64.0f) * EmSize + Position + new Vector2(0.0f, 4.0f);
            var box = new Box2(min, max);
            if (box.ContainsInclusive(mousePos))
                return line;
        }
        
        return null;
    }

    private int? CheckForCharacterCollision(SelectionLine line, Vector2 mousePos)
    {
        // Linear search
        // TODO: Turn this into a binary search

        if (line.SelectablePositions.Length <= 1)
            return null; // No characters to check

        for (int i = 0; i < line.SelectablePositions.Length - 1; i++)
        {
            var min = new Vector2(line.SelectablePositions[i] / 64.0f, line.Top / 64.0f) * EmSize + Position + new Vector2(0.0f, 4.0f);
            var max = new Vector2(line.SelectablePositions[i + 1] / 64.0f, line.Bottom / 64.0f) * EmSize + Position + new Vector2(0.0f, 4.0f);
            var box = new Box2(min, max);
            if (box.ContainsInclusive(mousePos))
                return line.SelectableIndices[i] + 1;
        }
        
        return null;
    }

    private void UpdateTextData()
    {
        selectionText = TextShaper.GetSelectionText(Font, Text, HorizontalAlignment, VerticalAlignment);
    }

    public void Render(RenderArgs args)
    {
        textEntity.Render(args);

        if (selectionText == null)
            return;
        
        foreach (var (bounding, index) in EnumerateBoxes(selectionText))
        {
            int selectionStart = Math.Min(selection.Item1, selection.Item2);
            int selectionEnd = Math.Max(selection.Item1, selection.Item2);
            
            if (index < selectionStart || index > selectionEnd)
                continue; // Not in selection range
            
            var min = bounding.Min;
            var max = bounding.Max;
            var pos = new Vector3(min.X, min.Y, 0.0f);
            var size = new Vector3(max.X - min.X, max.Y - min.Y, 1.0f);

            var matrixStack = args.MatrixStack;
            
            matrixStack.Push();
            matrixStack.Translate(0.5f, 0.5f, 0.0f);
            matrixStack.Scale(size);
            matrixStack.Translate(pos);

            meshEntity.Color = new Color4(100, 149, 237, 127);
            meshEntity.Render(args);
            
            matrixStack.Pop();
        }
    }

    private IEnumerable<(Box2 bounding, int index)> EnumerateBoxes(SelectionText selectionText)
    {
        foreach (var line in selectionText.Lines)
        {
            for (int i = 0; i < line.SelectablePositions.Length - 1; i++)
            {
                var min = new Vector2(line.SelectablePositions[i] / 64.0f, line.Top / 64.0f) * EmSize + new Vector2(0.0f, 4.0f);
                var max = new Vector2(line.SelectablePositions[i + 1] / 64.0f, line.Bottom / 64.0f) * EmSize + new Vector2(0.0f, 4.0f);
                var box = new Box2(min, max);
                yield return (box, line.SelectableIndices[i] + 1);
            }
        }
    } 
}