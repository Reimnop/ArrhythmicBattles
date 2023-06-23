using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public delegate void RenderCallback(Renderer renderer, CommandList commandList);

public class ViewportElement : VisualElement, IRenderable
{
    private readonly Renderer renderer;
    private readonly CommandList commandList = new();
    
    private readonly IRenderBuffer renderBuffer;
    private readonly RenderCallback renderCallback;

    private Box2 bounds;

    public ViewportElement(Renderer renderer, RenderCallback renderCallback)
    {
        this.renderer = renderer;
        this.renderCallback = renderCallback;
        renderBuffer = renderer.CreateRenderBuffer(Vector2i.One);
    }
    
    protected override void UpdateLayout(Box2 bounds)
    {
        this.bounds = bounds;
    }

    public override void Render(RenderArgs args)
    {
        commandList.Clear();
        renderCallback(renderer, commandList);
        renderer.Render((Vector2i) bounds.Size, commandList, renderBuffer);
        
        var transformation = args.MatrixStack.GlobalTransformation * args.CameraData.View * args.CameraData.Projection;
        var drawData = new RenderBufferDrawData(DefaultAssets.QuadMesh.ReadOnly, transformation, renderBuffer, PrimitiveType.Triangles);
        args.CommandList.AddDrawData(LayerType.Gui, drawData);
    }
}