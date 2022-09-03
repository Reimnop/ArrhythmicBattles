using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Rendering;

public class ScreenCapturer : IDisposable
{
    public int FramebufferHandle => framebuffer.Handle;
    public int ColorBufferHandle => colorBuffer.Handle;
    
    public int Width { get; }
    public int Height { get; }

    private readonly Framebuffer framebuffer;
    private readonly Renderbuffer depthBuffer;
    private readonly Texture2D colorBuffer;

    public ScreenCapturer(string name, int width, int height)
    {
        Width = width;
        Height = height;
        
        depthBuffer = new Renderbuffer($"{name}-depth", width, height, RenderbufferStorage.DepthComponent32f);
        colorBuffer = new Texture2D($"{name}-color", width, height, SizedInternalFormat.Rgba16f);
        
        framebuffer = new Framebuffer(name);
        framebuffer.Renderbuffer(FramebufferAttachment.DepthAttachment, depthBuffer);
        framebuffer.Texture(FramebufferAttachment.ColorAttachment0, colorBuffer);
    }

    public void Dispose()
    {
        framebuffer.Dispose();
        depthBuffer.Dispose();
        colorBuffer.Dispose();
    }
}