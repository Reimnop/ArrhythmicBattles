﻿using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;
using Textwriter;

namespace FlexFramework.Core.Entities;

public class TextEntity : Entity, IRenderable, IDisposable
{
    public Font Font
    {
        get => font;
        set
        {
            font = value;
            InvalidateMesh();
        }
    }

    public int BaselineOffset
    {
        get => baselineOffset;
        set
        {
            baselineOffset = value;
            InvalidateMesh();
        }
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get => horizontalAlignment;
        set
        {
            horizontalAlignment = value;
            InvalidateMesh();
        }
    }

    public VerticalAlignment VerticalAlignment
    {
        get => verticalAlignment;
        set
        {
            verticalAlignment = value;
            InvalidateMesh();
        }
    }

    public string Text
    {
        get => text;
        set
        {
            text = value;
            InvalidateMesh();
        }
    }

    public float EmSize { get; set; } = 1.0f;

    public Color4 Color { get; set; } = Color4.White;

    private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    private VerticalAlignment verticalAlignment = VerticalAlignment.Bottom;
    private int baselineOffset = 0;
    private Font font = null;
    private string text = "";

    private bool meshValid = false;

    private readonly FlexFrameworkMain engine;
    private readonly TextAssets textAssets;
    private readonly Mesh<TextVertexAdapter> mesh;
    
    private readonly List<TextVertex> vertices = new List<TextVertex>();

    public TextEntity(FlexFrameworkMain engine, Font font)
    {
        this.engine = engine;
        this.font = font;

        var textAssetsLocation = engine.DefaultAssets.TextAssets;
        textAssets = engine.ResourceRegistry.GetResource(textAssetsLocation);

        mesh = new Mesh<TextVertexAdapter>("text");
    }

    public void InvalidateMesh()
    {
        meshValid = false;
    }

    private void GenerateMesh()
    {
        TextBuilder builder = new TextBuilder(null, textAssets.Fonts)
            .WithBaselineOffset(baselineOffset)
            .WithHorizontalAlignment(horizontalAlignment)
            .WithVerticalAlignment(verticalAlignment)
            .AddText(new StyledText(text, font)
                .WithColor(System.Drawing.Color.White));
        
        TextMeshGenerator.GenerateVertices(builder.Build(), vertices);
        
        Span<TextVertexAdapter> vertexSpan = stackalloc TextVertexAdapter[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            vertexSpan[i] = new TextVertexAdapter(vertices[i]);
        }
        
        mesh.LoadData(vertexSpan);
    }

    public void Render(RenderArgs args)
    {
        if (!meshValid)
        {
            meshValid = true;
            GenerateMesh();
        }
        
        if (mesh.Count == 0)
        {
            return;
        }
        
        Renderer renderer = args.Renderer;
        int layerId = args.LayerId;
        MatrixStack matrixStack = args.MatrixStack;
        CameraData cameraData = args.CameraData;
        
        matrixStack.Push();
        matrixStack.Scale(EmSize, EmSize, 1.0f);

        Matrix4 transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        TextDrawData textDrawData = new TextDrawData(mesh.VertexArray, mesh.Count, transformation, Color, 4.0f * EmSize);
        
        renderer.EnqueueDrawData(layerId, textDrawData);
        
        matrixStack.Pop();
    }
    
    public void Dispose()
    {
        mesh.Dispose();
    }
}