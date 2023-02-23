﻿using FlexFramework.Core.Data;
using FlexFramework.Core;
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
    
    public Color4 Color { get; set; } = Color4.White;

    private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    private VerticalAlignment verticalAlignment = VerticalAlignment.Bottom;
    private int baselineOffset = 0;
    private Font font = null;
    private string text = "";

    private bool meshValid = false;

    private readonly FlexFrameworkMain engine;
    private readonly Mesh<TextVertexExtern> mesh;
    
    private readonly List<TextVertex> vertices = new List<TextVertex>();

    public TextEntity(FlexFrameworkMain engine, Font font)
    {
        this.engine = engine;
        this.font = font;
        HorizontalAlignment = horizontalAlignment;
        
        mesh = new Mesh<TextVertexExtern>("text");
    }

    public void InvalidateMesh()
    {
        meshValid = false;
    }

    private void GenerateMesh()
    {
        TextBuilder builder = new TextBuilder(font.Height, engine.TextResources.Fonts)
            .WithBaselineOffset(baselineOffset)
            .WithHorizontalAlignment(horizontalAlignment)
            .WithVerticalAlignment(verticalAlignment)
            .AddText(new StyledText(text, font)
                .WithColor(System.Drawing.Color.White));
        
        TextMeshGenerator.GenerateVertices(builder.Build(), vertices);
        
        Span<TextVertexExtern> vertexSpan = stackalloc TextVertexExtern[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            vertexSpan[i] = new TextVertexExtern(vertices[i]);
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

        Matrix4 transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        TextDrawData textDrawData = new TextDrawData(mesh.VertexArray, mesh.Count, transformation, Color);

        renderer.EnqueueDrawData(layerId, textDrawData);
    }
    
    public void Dispose()
    {
        mesh.Dispose();
    }
}