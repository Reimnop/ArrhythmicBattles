using System.Runtime.CompilerServices;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Text;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class TextEntity : Entity, IRenderable
{
    public int BaselineOffset { get; set; }

    public string Text
    {
        get => text;
        set
        {
            text = value;
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
    
    private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    private VerticalAlignment verticalAlignment = VerticalAlignment.Top;

    public float EmSize { get; set; } = 1.0f;

    public Color4 Color { get; set; } = Color4.White;
    
    private Font font;
    private string text = "";

    private bool meshValid = false;
    
    private readonly Mesh<TextVertex> mesh;
    private readonly Texture fontAtlas;
    private readonly MeshGenerator meshGenerator = new();

    public TextEntity(Font font)
    {
        this.font = font;
        fontAtlas = new Texture("font_atlas", font.Texture.Width, font.Texture.Height, PixelFormat.Rgb32f);
        fontAtlas.SetData<Rgb32f>(font.Texture.Pixels);

        VertexLayout vertexLayout = new VertexLayout(
            Unsafe.SizeOf<TextVertex>(),
            new VertexAttribute(VertexAttributeIntent.Position, VertexAttributeType.Float, 2, 0),
            new VertexAttribute(VertexAttributeIntent.TexCoord0, VertexAttributeType.Float, 2, 2 * sizeof(float))
        );

        mesh = new Mesh<TextVertex>("text", vertexLayout);
    }

    public void InvalidateMesh()
    {
        meshValid = false;
    }

    private void GenerateMesh()
    {
        var shapedText = TextShaper.ShapeText(font, text, horizontalAlignment, verticalAlignment);
        var vertexSpan = meshGenerator.GenerateMesh(shapedText);

        mesh.SetData(vertexSpan, null);
    }

    public void Render(RenderArgs args)
    {
        if (!meshValid)
        {
            meshValid = true;
            GenerateMesh();
        }

        CommandList commandList = args.CommandList;
        LayerType layerType = args.LayerType;
        MatrixStack matrixStack = args.MatrixStack;
        CameraData cameraData = args.CameraData;
        
        matrixStack.Push();
        matrixStack.Scale(EmSize, EmSize, 1.0f);
        matrixStack.Translate(0.0f, BaselineOffset / 64.0f, 0.0f);

        Matrix4 transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        TextDrawData textDrawData = new TextDrawData(mesh.ReadOnly, fontAtlas.ReadOnly, transformation, Color, 4.0f * EmSize);
        
        commandList.AddDrawData(layerType, textDrawData);
        matrixStack.Pop();
    }
}