﻿using System.ComponentModel;
using System.Runtime.InteropServices;
using FlexFramework.Core.Audio;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Util.Exceptions;
using FlexFramework.Util.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework;

public delegate Renderer RendererFactory(FlexFrameworkMain engine);
public delegate void LogCallbackDelegate(LogLevel level, string name, string message, Exception? exception);

/// <summary>
/// Main class for the FlexFramework
/// </summary>
public class FlexFrameworkMain : NativeWindow, ILoggerFactory
{
    private class FlexFrameworkLogger : ILogger
    {
        private readonly FlexFrameworkMain engine;
        private readonly string name;
        
        public FlexFrameworkLogger(FlexFrameworkMain engine, string name)
        {
            this.engine = engine;
            this.name = name;
        }
        
        public void Log(LogLevel level, string message, Exception? exception = null)
        {
            engine.logCallback?.Invoke(level, name, message, exception);
        }
    }
    
    /// <summary>
    /// Input manager for the engine
    /// </summary>
    public Input Input { get; }
    
    /// <summary>
    /// Current renderer for rendering objects
    /// </summary>
    public Renderer Renderer { get; }
    
    private readonly SceneManager sceneManager;
    private readonly AudioManager audioManager;
    
    private readonly ILogger logger;
    private readonly LogCallbackDelegate? logCallback;

    private float time;

#if DEBUG
    // This causes memory leaks, but the method needs to be pinned to prevent garbage collection
    private GCHandle leakedGcHandle;
#endif

    public FlexFrameworkMain(
        NativeWindowSettings nws, 
        RendererFactory rendererFactory, 
        LogCallbackDelegate? logCallback = null) : base(nws)
    {
        this.logCallback = logCallback;
        logger = this.CreateLogger<FlexFrameworkMain>();
        
#if DEBUG
        // init GL debug callback
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        
        DebugProc debugProc = LogGlMessage;
        leakedGcHandle = GCHandle.Alloc(debugProc);
        GL.DebugMessageCallback(debugProc, IntPtr.Zero);
#endif

        sceneManager = new SceneManager(this);
        audioManager = new AudioManager();
        Input = new Input(this);

        Renderer = rendererFactory(this);
        logger.LogInfo($"Initialized renderer [{Renderer.GetType().Name}]");
    }
    
    public ILogger GetLogger(string name)
    {
        return new FlexFrameworkLogger(this, name);
    }

#if DEBUG
    private void LogGlMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
        var messageString = Marshal.PtrToStringAnsi(message, length);
        var logLevel = severity switch
        {
            DebugSeverity.DontCare => LogLevel.Verbose,
            DebugSeverity.DebugSeverityNotification => LogLevel.Debug,
            DebugSeverity.DebugSeverityHigh => LogLevel.Error,
            DebugSeverity.DebugSeverityMedium => LogLevel.Warning,
            DebugSeverity.DebugSeverityLow => LogLevel.Info,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };

        logger.Log(logLevel, messageString);
    }
#endif

    public Scene LoadScene(Scene scene)
    {
        return sceneManager.LoadScene(scene);
    }

    public void Update()
    {
        ProcessInputEvents();
        ProcessWindowEvents(false);

        var currentTime = (float) GLFW.GetTime();
        var deltaTime = currentTime - time;
        time = currentTime;

        Tick(deltaTime);
        Render();
    }

    private void Tick(float deltaTime)
    {
        if (sceneManager.CurrentScene == null)
        {
            throw new NoSceneException();
        }

        var args = new UpdateArgs(time, deltaTime);
        sceneManager.CurrentScene.Update(args);
        Renderer.Update(args);
    }

    private unsafe void Render()
    {
        if (Renderer == null)
        {
            throw new NoRendererException();
        }
        
        sceneManager.CurrentScene.Render(Renderer);
    }

    public unsafe void Present(IRenderBuffer buffer)
    {
        buffer.BlitToBackBuffer(ClientSize);
        GLFW.SwapBuffers(WindowPtr);
    }

    public unsafe bool ShouldClose()
    {
        return GLFW.WindowShouldClose(WindowPtr);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

#if DEBUG
        // Unleak the debug callback
        leakedGcHandle.Free();
#endif
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        
        audioManager.Dispose();
        if (Renderer is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
