using System.Diagnostics;
using System.Drawing;
using FlexFramework.Core.Util;
using FlexFramework.Rendering.Data;
using FlexFramework.Rendering.DefaultRenderingStrategies;
using FlexFramework.Rendering.PostProcessing;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Rendering;

public class DefaultRenderer : Renderer
{
    public const string OpaqueLayerName = "opaque";
    public const string AlphaClipLayerName = "alphaclip";
    public const string TransparentLayerName = "transparent";
    public const string GuiLayerName = "gui";
    
    private Registry<string, List<IDrawData>> renderLayerRegistry = new Registry<string, List<IDrawData>>();
    private Dictionary<Type, RenderingStrategy> renderingStrategies = new Dictionary<Type, RenderingStrategy>();

    private List<PostProcessor> postProcessors = new List<PostProcessor>();

    private ScreenCapturer? screenCapturer;

    private GLStateManager stateManager;
    private ShaderProgram unlitShader;
    private ShaderProgram litShader;

    private int opaqueLayerId;
    private int alphaClipLayerId;
    private int transparentLayerId;
    private int guiLayerId;

    public override void Init()
    {
        stateManager = new GLStateManager();
        
        // Init GL objects
        unlitShader = LoadProgram("unlit", "Assets/Shaders/unlit");
        litShader = LoadProgram("lit", "Assets/Shaders/lit");

        // Register render layers
        opaqueLayerId = RegisterLayer(OpaqueLayerName);
        alphaClipLayerId = RegisterLayer(AlphaClipLayerName);
        transparentLayerId = RegisterLayer(TransparentLayerName);
        guiLayerId = RegisterLayer(GuiLayerName);
        renderLayerRegistry.Freeze();
        
        // Register render strategies
        RegisterRenderingStrategy<VertexDrawData, VertexRenderStrategy>(unlitShader);
        RegisterRenderingStrategy<IndexedVertexDrawData, IndexedVertexRenderStrategy>(unlitShader);
        RegisterRenderingStrategy<LitVertexDrawData, LitVertexRenderStrategy>(litShader);
        RegisterRenderingStrategy<SkinnedVertexDrawData, SkinnedVertexRenderStrategy>();
        RegisterRenderingStrategy<TextDrawData, TextRenderStrategy>(Engine);
        RegisterRenderingStrategy<CustomDrawData, CustomRenderStrategy>();

        GL.CullFace(CullFaceMode.Back);
        GL.FrontFace(FrontFaceDirection.Ccw);
    }

    private void RegisterRenderingStrategy<TDrawData, TStrategy>(params object?[]? args) 
        where TDrawData : IDrawData 
        where TStrategy : RenderingStrategy
    {
        TStrategy? strategy = Activator.CreateInstance(typeof(TStrategy), args) as TStrategy;
        if (strategy is null)
        {
            throw new ArgumentException();
        }
        renderingStrategies.Add(typeof(TDrawData), strategy);
    }

    private int RegisterLayer(string name)
    {
        return renderLayerRegistry.Register(name, () => new List<IDrawData>());
    }

    private ShaderProgram LoadProgram(string name, string path)
    {
        using Shader vertexShader = new Shader($"{name}-vs", File.ReadAllText($"{path}.vert"), ShaderType.VertexShader);
        using Shader fragmentShader = new Shader($"{name}-fs", File.ReadAllText($"{path}.frag"), ShaderType.FragmentShader);

        ShaderProgram program = new ShaderProgram(name);
        program.LinkShaders(vertexShader, fragmentShader);

        return program;
    }

    public override int GetLayerId(string name)
    {
        return renderLayerRegistry.GetId(name);
    }

    public override void EnqueueDrawData(int layerId, IDrawData drawData)
    {
        if (!renderLayerRegistry.HasKey(layerId))
        {
            return;
        }
        
        renderLayerRegistry[layerId].Add(drawData);
    }

    public override void UsePostProcessor(PostProcessor postProcessor)
    {
        postProcessors.Add(postProcessor);
    }

    private bool ShouldUpdateCapturer(Vector2i size)
    {
        if (screenCapturer == null)
        {
            return true;
        }
        
        if (screenCapturer.Width != size.X || screenCapturer.Height != size.Y)
        {
            return true;
        }

        return false;
    }

    public override void Update(UpdateArgs args)
    {
        Vector2i size = Engine.ClientSize;
        if (ShouldUpdateCapturer(size))
        {
            screenCapturer?.Dispose();
            screenCapturer = new ScreenCapturer("scene", size.X, size.Y);
        }
    }

    public override void Render()
    {
        Debug.Assert(screenCapturer != null);
        
        stateManager.BindFramebuffer(screenCapturer.Framebuffer.Handle);

        GL.Viewport(0, 0, screenCapturer.Width, screenCapturer.Height);
        
        GL.ClearColor(ClearColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // stateManager.SetCapability(EnableCap.Multisample, true);

        using TemporaryList<IDrawData> opaqueLayer = renderLayerRegistry[opaqueLayerId];
        using TemporaryList<IDrawData> alphaClipLayer = renderLayerRegistry[alphaClipLayerId];
        using TemporaryList<IDrawData> transparentLayer = renderLayerRegistry[transparentLayerId];
        using TemporaryList<IDrawData> guiLayer = renderLayerRegistry[guiLayerId];
        
        stateManager.SetCapability(EnableCap.DepthTest, true);
        stateManager.SetCapability(EnableCap.CullFace, true);
        GL.DepthMask(true);
        RenderLayer(opaqueLayer);
        
        stateManager.SetCapability(EnableCap.CullFace, false);
        RenderLayer(alphaClipLayer);
        
        GL.DepthMask(false);
        stateManager.SetCapability(EnableCap.Blend, true);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        RenderLayer(transparentLayer);
        
        GL.DepthMask(true);
        
        stateManager.SetCapability(EnableCap.DepthTest, false);
        RenderLayer(guiLayer);
        stateManager.SetCapability(EnableCap.Blend, false);
        
        stateManager.BindFramebuffer(0);

        using TemporaryList<PostProcessor> postProcessors = this.postProcessors;
        RunPostProcessors(postProcessors, stateManager, screenCapturer.ColorBuffer);

        // Blit to backbuffer
        GL.ClearColor(Color.Black);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.BlitNamedFramebuffer(screenCapturer.Framebuffer.Handle, 0, 
            0, 0, screenCapturer.Width, screenCapturer.Height, 
            0, 0, Engine.ClientSize.X, Engine.ClientSize.Y,
            ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
    }

    private void RunPostProcessors(List<PostProcessor> postProcessors, GLStateManager stateManager, Texture2D texture)
    {
        Vector2i size = new Vector2i(texture.Width, texture.Height);
        postProcessors.ForEach(processor =>
        {
            if (processor.CurrentSize != size)
            {
                processor.Resize(size.X, size.Y);
            }
        });
        postProcessors.ForEach(processor => processor.Process(stateManager, texture));
    }

    private void RenderLayer(List<IDrawData> layer)
    {
        foreach (IDrawData drawData in layer)
        {
            RenderingStrategy strategy = renderingStrategies[drawData.GetType()];
            strategy.Draw(stateManager, drawData);
        }
    }

    public override void Dispose()
    {
        unlitShader.Dispose();

        foreach (var (_, strategy) in renderingStrategies)
        {
            strategy.Dispose();
        }
    }
}