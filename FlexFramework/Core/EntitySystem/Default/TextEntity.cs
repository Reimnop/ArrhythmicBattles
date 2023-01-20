using FlexFramework.Core.Data;
using FlexFramework.Core.Util;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;
using Textwriter;

namespace FlexFramework.Core.EntitySystem.Default;

public class TextEntity : Entity, IRenderable
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

    public float BaselineOffset
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
    private float baselineOffset = 0.0f;
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
            .WithBaselineOffset((int) (baselineOffset * 64))
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

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
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

        Matrix4 transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        TextDrawData textDrawData = new TextDrawData(mesh.VertexArray, mesh.Count, transformation, Color);

        renderer.EnqueueDrawData(layerId, textDrawData);
    }
    
    public override void Dispose()
    {
        mesh.Dispose();
    }
}