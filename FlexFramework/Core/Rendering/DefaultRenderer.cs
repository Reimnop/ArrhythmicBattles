using System.Diagnostics;
using System.Drawing;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.DefaultRenderingStrategies;
using FlexFramework.Core.Rendering.PostProcessing;
using FlexFramework.Core.Util;
using FlexFramework.Logging;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering;

public class DefaultRenderer : Renderer
{
    public const string OpaqueLayerName = "opaque";
    public const string AlphaClipLayerName = "alphaclip";
    public const string TransparentLayerName = "transparent";
    public const string GuiLayerName = "gui";
    
    private Registry<string, List<IDrawData>> renderLayerRegistry = new Registry<string, List<IDrawData>>();
    private Dictionary<Type, RenderingStrategy> renderingStrategies = new Dictionary<Type, RenderingStrategy>();

    private List<PostProcessor> postProcessors = new List<PostProcessor>();

    private ScreenCapturer? worldScreenCapturer;
    private ScreenCapturer? guiScreenCapturer;

    private GLStateManager stateManager;
    private ShaderProgram unlitShader;
    private ShaderProgram litShader;
    private ShaderProgram skyboxShader;

    private Texture2D? skyboxTexture;
    private CameraData skyboxCameraData;

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
        skyboxShader = LoadComputeProgram("skybox", "Assets/Shaders/Compute/skybox");

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
        
        // Set GL modes
        GL.CullFace(CullFaceMode.Back);
        GL.FrontFace(FrontFaceDirection.Ccw);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private void RegisterRenderingStrategy<TDrawData, TStrategy>(params object?[]? args) 
        where TDrawData : IDrawData 
        where TStrategy : RenderingStrategy
    {
        Engine.LogMessage(this, Severity.Info, null, $"Initializing rendering strategy [{typeof(TStrategy).FullName}]");
        
        TStrategy? strategy = Activator.CreateInstance(typeof(TStrategy), args) as TStrategy;
        if (strategy is null)
        {
            throw new ArgumentException();
        }
        renderingStrategies.Add(typeof(TDrawData), strategy);
    }

    private int RegisterLayer(string name)
    {
        Engine.LogMessage(this, Severity.Info, null, $"Initializing render layer [{name}]");
        
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
    
    private ShaderProgram LoadComputeProgram(string name, string path)
    {
        using Shader shader = new Shader($"{name}-vs", File.ReadAllText($"{path}.comp"), ShaderType.ComputeShader);

        ShaderProgram program = new ShaderProgram(name);
        program.LinkShaders(shader);

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

    public override void UseSkybox(Texture2D skyboxTexture, CameraData cameraData)
    {
        this.skyboxTexture = skyboxTexture;
        skyboxCameraData = cameraData;
    }

    private bool ShouldUpdateCapturer(Vector2i size, ScreenCapturer? capturer)
    {
        if (capturer == null)
        {
            return true;
        }
        
        if (capturer.Width != size.X || capturer.Height != size.Y)
        {
            return true;
        }

        return false;
    }

    public override void Update(UpdateArgs args)
    {
        Vector2i size = Engine.ClientSize;
        
        if (ShouldUpdateCapturer(size, worldScreenCapturer))
        {
            worldScreenCapturer?.Dispose();
            worldScreenCapturer = new ScreenCapturer("world", size.X, size.Y);
        }
        
        if (ShouldUpdateCapturer(size, guiScreenCapturer))
        {
            guiScreenCapturer?.Dispose();
            guiScreenCapturer = new ScreenCapturer("gui", size.X, size.Y, false);
        }
    }

    public override void Render()
    {
        Debug.Assert(worldScreenCapturer != null);
        Debug.Assert(guiScreenCapturer != null);
        
        stateManager.BindFramebuffer(worldScreenCapturer.FrameBuffer.Handle); // Bind world framebuffer

        GL.Viewport(0, 0, worldScreenCapturer.Width, worldScreenCapturer.Height);
        
        GL.ClearColor(ClearColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        // Render skybox
        if (skyboxTexture != null)
        {
            stateManager.UseProgram(skyboxShader.Handle);
            stateManager.BindTextureUnit(0, skyboxTexture.Handle);
            
            Matrix4 inverseView = Matrix4.Invert(skyboxCameraData.View);
            Matrix4 inverseProjection = Matrix4.Invert(skyboxCameraData.Projection);
            
            GL.UniformMatrix4(1, true, ref inverseProjection);
            GL.UniformMatrix4(2, true, ref inverseView);
            
            GL.BindImageTexture(0, worldScreenCapturer.ColorBuffer.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
            GL.DispatchCompute(DivideIntCeil(worldScreenCapturer.ColorBuffer.Width, 8), DivideIntCeil(worldScreenCapturer.ColorBuffer.Height, 8), 1);
            
            skyboxTexture = null;
        }

        using TemporaryList<IDrawData> opaqueLayer = renderLayerRegistry[opaqueLayerId];
        using TemporaryList<IDrawData> alphaClipLayer = renderLayerRegistry[alphaClipLayerId];
        using TemporaryList<IDrawData> transparentLayer = renderLayerRegistry[transparentLayerId];
        using TemporaryList<IDrawData> guiLayer = renderLayerRegistry[guiLayerId];
        
        stateManager.SetCapability(EnableCap.DepthTest, true);
        stateManager.SetCapability(EnableCap.CullFace, true);
        stateManager.SetCapability(EnableCap.Blend, false);
        stateManager.SetDepthMask(true);
        RenderLayer(opaqueLayer);
        
        stateManager.SetCapability(EnableCap.DepthTest, true);
        stateManager.SetCapability(EnableCap.CullFace, false);
        stateManager.SetCapability(EnableCap.Blend, false);
        stateManager.SetDepthMask(true);
        RenderLayer(alphaClipLayer);
        
        stateManager.SetCapability(EnableCap.DepthTest, true);
        stateManager.SetCapability(EnableCap.CullFace, false);
        stateManager.SetCapability(EnableCap.Blend, true);
        stateManager.SetDepthMask(false);
        RenderLayer(transparentLayer);
        
        // Post-process world framebuffer
        using TemporaryList<PostProcessor> postProcessors = this.postProcessors;
        RunPostProcessors(postProcessors, stateManager, worldScreenCapturer.ColorBuffer);
        
        stateManager.BindFramebuffer(guiScreenCapturer.FrameBuffer.Handle); // Finish rendering world, bind gui framebuffer
        
        // Blit world framebuffer to gui framebuffer
        GL.ClearColor(Color.Black);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.BlitNamedFramebuffer(worldScreenCapturer.FrameBuffer.Handle, guiScreenCapturer.FrameBuffer.Handle, 
            0, 0, worldScreenCapturer.Width, worldScreenCapturer.Height, 
            0, 0, guiScreenCapturer.Width, guiScreenCapturer.Height,
            ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

        stateManager.SetCapability(EnableCap.DepthTest, false);
        stateManager.SetCapability(EnableCap.CullFace, false);
        stateManager.SetCapability(EnableCap.Blend, true);
        stateManager.SetDepthMask(true);
        RenderLayer(guiLayer);

        stateManager.BindFramebuffer(0); // Finally, bind default framebuffer


        // Blit to backbuffer
        GL.ClearColor(Color.Black);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.BlitNamedFramebuffer(guiScreenCapturer.FrameBuffer.Handle, 0, 
            0, 0, guiScreenCapturer.Width, guiScreenCapturer.Height, 
            0, 0, Engine.ClientSize.X, Engine.ClientSize.Y,
            ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
    }

    private void RunPostProcessors(List<PostProcessor> postProcessors, GLStateManager stateManager, Texture2D texture)
    {
        Vector2i size = new Vector2i(texture.Width, texture.Height);
        postProcessors.ForEach(processor =>
        {
            if (processor.CurrentSize == Vector2i.Zero)
            {
                Engine.LogMessage(this, Severity.Info, null, $"Initializing post processor [{processor}] with size [{size}]");
                processor.Init(size);
                return;
            }
            
            if (processor.CurrentSize != size)
            {
                Engine.LogMessage(this, Severity.Info, null, $"Resizing post processor [{processor}] from [{processor.CurrentSize}] to [{size}]");
                processor.Resize(size);
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
        litShader.Dispose();
        skyboxShader.Dispose();
        worldScreenCapturer?.Dispose();
        guiScreenCapturer?.Dispose();

        foreach (var (_, strategy) in renderingStrategies)
        {
            strategy.Dispose();
        }
    }
    
    private static int DivideIntCeil(int a, int b)
    {
        return a / b + (a % b > 0 ? 1 : 0);
    }
}