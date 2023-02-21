using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;
using Textwriter;

namespace FlexFramework.Core.UserInterface.Drawables;

public class TextDrawable : Drawable
{
    private readonly Bounds bounds;
    
    private Mesh<TextVertexExtern> mesh;
    
    public TextDrawable(FlexFrameworkMain engine, Bounds bounds, string text, Color4 color, Font font) : base(bounds, null)
    {
        this.bounds = bounds;
        
        TextBuilder textBuilder = new TextBuilder(font.Height, engine.TextResources.Fonts)
            .WithBaselineOffset(font.TotalHeight)
            .AddText(new StyledText(text, font)
                .WithColor(System.Drawing.Color.FromArgb((int) (color.A * 255), (int) (color.R * 255), (int) (color.G * 255), (int) (color.B * 255))));

        BuiltText builtText = textBuilder.Build();

        List<TextVertex> textVertices = new List<TextVertex>();
        TextMeshGenerator.GenerateVertices(builtText, textVertices);
        
        Span<TextVertexExtern> vertexSpan = stackalloc TextVertexExtern[textVertices.Count];
        for (int i = 0; i < textVertices.Count; i++)
        {
            vertexSpan[i] = new TextVertexExtern(textVertices[i]);
        }
        
        mesh = new Mesh<TextVertexExtern>("text", vertexSpan);
    }

    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        matrixStack.Push();
        Transform.ApplyToMatrixStack(matrixStack);
        matrixStack.Push();
        matrixStack.Translate(bounds.X0, bounds.Y0, 0.0f);
        Matrix4 transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        TextDrawData textDrawData = new TextDrawData(mesh.VertexArray, mesh.Count, transformation, Color4.White);
        renderer.EnqueueDrawData(layerId, textDrawData);
        matrixStack.Pop();
        matrixStack.Pop();
    }
}