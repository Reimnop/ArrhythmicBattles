using System.ComponentModel;
using System.Runtime.InteropServices;
using FlexFramework.Core.Audio;
using FlexFramework.Core;
using FlexFramework.Core.Util;
using FlexFramework.Logging;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core.Rendering.Text;
using FlexFramework.Util.Exceptions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework;

public class FlexFrameworkMain : NativeWindow
{
    public Renderer Renderer { get; private set; } = null!;
    public TextResources TextResources { get; private set; } = null!;
    public PersistentResources PersistentResources { get; }
    public SceneManager SceneManager { get; }
    public AudioManager AudioManager { get; }
    public Input Input { get; }
    
    public bool ClampDeltaTime { get; set; } = true;

    public event LogEventHandler? Log;

    private float time = 0.0f;

#if DEBUG
    // This causes memory leaks, but the method needs to be pinned to prevent garbage collection
    private GCHandle leakedGcHandle;
#endif

    public FlexFrameworkMain(NativeWindowSettings nws) : base(nws)
    {
        Context.MakeCurrent();

#if DEBUG
        // init GL debug callback
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        
        DebugProc debugProc = LogGlMessage;
        leakedGcHandle = GCHandle.Alloc(debugProc);
        GL.DebugMessageCallback(debugProc, IntPtr.Zero);
#endif

        SceneManager = new SceneManager(this);
        PersistentResources = new PersistentResources();
        AudioManager = new AudioManager();
        Input = new Input(this);
    }

    internal void LogMessage(object? sender, Severity severity, string? type, string message)
    {
        if (sender == null)
        {
            sender = this;
        }
        
        Log?.Invoke(sender, new LogEventArgs(severity, type, message));
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

    public Renderer UseRenderer(Renderer renderer)
    {
        LogMessage(null, Severity.Info, null, $"Using renderer [{renderer}]");
        
        if (Renderer != null)
        {
            Renderer.Dispose();
        }
        
        renderer.SetEngine(this);
        renderer.Init();

        Renderer = renderer;
        return renderer;
    }
    
    public T UseRenderer<T>(params object?[]? args) where T : Renderer
    {
        T? renderer = (T?) Activator.CreateInstance(typeof(T), args);

        if (renderer == null)
        {
            throw new LoadRendererException(typeof(T));
        }

        return (T) UseRenderer(renderer);
    }

    public TextResources LoadFonts(int atlasWidth, params FontFileInfo[] fontFiles)
    {
        LogMessage(null, Severity.Info, null, $"Loading font atlas(es) with width [{atlasWidth}]");
        
        if (TextResources != null)
        {
            TextResources.Dispose();
        }

        TextResources = new TextResources(atlasWidth, fontFiles);
        return TextResources;
    }

    public Scene LoadScene(Scene scene)
    {
        return SceneManager.LoadScene(scene);
    }

    public T LoadScene<T>(params object?[]? args) where T : Scene
    {
        return SceneManager.LoadScene<T>(args);
    }

    public void Update()
    {
        ProcessInputEvents();
        ProcessWindowEvents(false);

        float currentTime = (float) GLFW.GetTime();
        float deltaTime = currentTime - time;
        time = currentTime;

        if (deltaTime > 1.0f)
        {
            LogMessage(null, Severity.Warning, null, $"Last frame took [{deltaTime * 1000.0f}ms]! Is the thread being blocked?");
        }
        
        // clamp delta time to 1/10th of a second
        if (ClampDeltaTime)
        {
            deltaTime = Math.Clamp(deltaTime, 0.0f, 0.1f);
        }

        Tick(deltaTime);
        Render();
    }

    private void Tick(float deltaTime)
    {
        if (SceneManager.CurrentScene == null)
        {
            throw new NoSceneException();
        }

        UpdateArgs args = new UpdateArgs(time, deltaTime);

        SceneManager.CurrentScene.Update(args);
        SceneManager.CurrentScene.UpdateInternal(args);
        
        Renderer.Update(args);
    }

    private unsafe void Render()
    {
        if (Renderer == null)
        {
            throw new NoRendererException();
        }
        
        SceneManager.CurrentScene.Render(Renderer);
        Renderer.Render();
        
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
        leakedGcHandle.Free();
#endif
    }
}