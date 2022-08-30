using FlexFramework.Core.Util;
using FlexFramework.Rendering.Data;
using FlexFramework.Rendering.DefaultRenderingStrategies;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Rendering;

public class DefaultRenderer : Renderer
{
    public const string OpaqueLayerName = "opaque";
    public const string AlphaClipLayerName = "alphaclip";
    public const string TransparentLayerName = "transparent";
    public const string GuiLayerName = "gui";
    
    private Registry<string, List<IDrawData>> renderLayerRegistry = new Registry<string, List<IDrawData>>();
    private Dictionary<Type, RenderingStrategy> renderingStrategies = new Dictionary<Type, RenderingStrategy>();

    private GLStateManager stateManager;
    private ShaderProgram unlitShader;

    private int opaqueLayerId;
    private int alphaClipLayerId;
    private int transparentLayerId;
    private int guiLayerId;

    public override void Init()
    {
        stateManager = new GLStateManager();
        unlitShader = LoadProgram("unlit", "Assets/Shaders/unlit");
        
        // Register render layers
        opaqueLayerId = RegisterLayer(OpaqueLayerName);
        alphaClipLayerId = RegisterLayer(AlphaClipLayerName);
        transparentLayerId = RegisterLayer(TransparentLayerName);
        guiLayerId = RegisterLayer(GuiLayerName);
        renderLayerRegistry.Freeze();
        
        // Register render strategies
        RegisterRenderingStrategy<VertexDrawData, VertexRenderStrategy>(unlitShader);
        RegisterRenderingStrategy<IndexedVertexDrawData, IndexedVertexRenderStrategy>(unlitShader);
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

    public override void Update(UpdateArgs args)
    {
    }

    public override void Render()
    {
        GL.ClearColor(ClearColor);
        
        GL.Viewport(0, 0, Engine.ClientSize.X, Engine.ClientSize.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        stateManager.SetCapability(EnableCap.Multisample, true);

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