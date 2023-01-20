﻿using System.Diagnostics;
using System.Drawing;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.Strategy;
using FlexFramework.Core.Rendering.PostProcessing;
using FlexFramework.Core.Util;
using FlexFramework.Logging;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering;

public class DefaultRenderer : Renderer, ILighting
{
    public const string OpaqueLayerName = "opaque";
    public const string AlphaClipLayerName = "alphaclip";
    public const string TransparentLayerName = "transparent";
    public const string GuiLayerName = "gui";

    public Vector3 AmbientLight { get; set; } = Vector3.One * 0.4f;
    public DirectionalLight? DirectionalLight { get; set; }

    public override GpuInfo GpuInfo => gpuInfo;
    private GpuInfo gpuInfo = null!;

    private Registry<string, List<IDrawData>> renderLayerRegistry = new Registry<string, List<IDrawData>>();
    private Dictionary<Type, RenderStrategy> renderStrategies = new Dictionary<Type, RenderStrategy>();

    private List<PostProcessor> postProcessors = new List<PostProcessor>();

    private ScreenCapturer? worldScreenCapturer;
    private ScreenCapturer? guiScreenCapturer;

    private GLStateManager stateManager;
    private ShaderProgram unlitShader;
    private ShaderProgram litShader;
    private ShaderProgram skyboxShader;

    private int opaqueLayerId;
    private int alphaClipLayerId;
    private int transparentLayerId;
    private int guiLayerId;

    public override void Init()
    {
        stateManager = new GLStateManager();
        
        // Set GpuInfo
        string name = GL.GetString(StringName.Renderer);
        string vendor = GL.GetString(StringName.Vendor);
        string version = GL.GetString(StringName.Version);
        gpuInfo = new GpuInfo(name, vendor, version);
        
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
        RegisterRenderStrategy<VertexDrawData>(new VertexRenderStrategy(unlitShader));
        RegisterRenderStrategy<IndexedVertexDrawData>(new IndexedVertexRenderStrategy(unlitShader));
        RegisterRenderStrategy<LitVertexDrawData>(new LitVertexRenderStrategy(this, litShader));
        RegisterRenderStrategy<SkinnedVertexDrawData>(new SkinnedVertexRenderStrategy(this));
        RegisterRenderStrategy<TextDrawData>(new TextRenderStrategy(Engine));
        RegisterRenderStrategy<CustomDrawData>(new CustomRenderStrategy());
        
        // Set GL modes
        GL.CullFace(CullFaceMode.Back);
        GL.FrontFace(FrontFaceDirection.Ccw);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private void RegisterRenderStrategy<TDrawData>(RenderStrategy strategy) where TDrawData : IDrawData
    {
        Engine.LogMessage(this, Severity.Info, null, $"Initializing render strategy [{strategy.GetType().Name}] for [{typeof(TDrawData).Name}]");
        renderStrategies.Add(typeof(TDrawData), strategy);
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
                Engine.LogMessage(this, Severity.Info, null, $"Initializing post processor [{processor.GetType().Name}] with size {size}");
                processor.Init(size);
                return;
            }
            
            if (processor.CurrentSize != size)
            {
                Engine.LogMessage(this, Severity.Info, null, $"Resizing post processor [{processor.GetType().Name}] from {processor.CurrentSize} to {size}");
                processor.Resize(size);
            }
        });
        postProcessors.ForEach(processor => processor.Process(stateManager, texture));
    }

    private void RenderLayer(List<IDrawData> layer)
    {
        foreach (IDrawData drawData in layer)
        {
            RenderStrategy strategy = renderStrategies[drawData.GetType()];
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

        foreach (var (_, strategy) in renderStrategies)
        {
            strategy.Dispose();
        }
    }
    
    private static int DivideIntCeil(int a, int b)
    {
        return a / b + (a % b > 0 ? 1 : 0);
    }
}