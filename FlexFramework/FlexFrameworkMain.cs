using System.ComponentModel;
using System.Runtime.InteropServices;
using FlexFramework.Core.Audio;
using FlexFramework.Core;
using FlexFramework.Logging;
using FlexFramework.Core.Rendering;
using FlexFramework.Util.Exceptions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework;

public delegate Renderer RendererFactory(FlexFrameworkMain engine);
public delegate void LogCallbackDelegate(object sender, LogEventArgs args);

/// <summary>
/// Main class for the FlexFramework
/// </summary>
public class FlexFrameworkMain : NativeWindow
{
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
    
    private LogCallbackDelegate? logCallback;

    private float time;

#if DEBUG
    // This causes memory leaks, but the method needs to be pinned to prevent garbage collection
    private GCHandle leakedGcHandle;
#endif

    public FlexFrameworkMain(NativeWindowSettings nws, RendererFactory rendererFactory, LogCallbackDelegate? logCallback = null) : base(nws)
    {
        this.logCallback = logCallback;
        
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
        LogMessage(null, Severity.Info, null, $"Initialized renderer [{Renderer.GetType().Name}]");
    }

    internal void LogMessage(object? sender, Severity severity, string? type, string message)
    {
        sender ??= this;
        logCallback?.Invoke(sender, new LogEventArgs(severity, type, message));
    }

#if DEBUG
    private void LogGlMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
        if (severity == DebugSeverity.DebugSeverityNotification)
        {
            return;
        }
        
        string messageString = Marshal.PtrToStringAnsi(message, length);
        
        Severity severityEnum;
        switch (severity)
        {
            case DebugSeverity.DebugSeverityHigh:
                severityEnum = Severity.Error;
                break;
            case DebugSeverity.DebugSeverityMedium:
                severityEnum = Severity.Warning;
                break;
            case DebugSeverity.DebugSeverityLow:
                severityEnum = Severity.Info;
                break;
            default:
                severityEnum = Severity.Debug;
                break;
        }
        
        LogMessage(null, severityEnum, "OpenGL", messageString);
        
        if (type == DebugType.DebugTypeError)
        {
            throw new Exception(messageString);
        }
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