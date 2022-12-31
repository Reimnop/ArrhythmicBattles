using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Rendering;

public class ScreenCapturer : IDisposable
{
    public int Width { get; }
    public int Height { get; }

    public Framebuffer FrameBuffer { get; }
    public Renderbuffer DepthBuffer { get; }
    public Texture2D ColorBuffer { get; }

    public ScreenCapturer(string name, int width, int height)
    {
        Width = width;
        Height = height;
        
        DepthBuffer = new Renderbuffer($"{name}-depth", width, height, RenderbufferStorage.DepthComponent32f);
        ColorBuffer = new Texture2D($"{name}-color", width, height, SizedInternalFormat.Rgba16f);
        ColorBuffer.SetMinFilter(TextureMinFilter.Linear);
        ColorBuffer.SetMagFilter(TextureMagFilter.Linear);
        ColorBuffer.SetWrapS(TextureWrapMode.ClampToEdge);
        ColorBuffer.SetWrapT(TextureWrapMode.ClampToEdge);

        FrameBuffer = new Framebuffer(name);
        FrameBuffer.Renderbuffer(FramebufferAttachment.DepthAttachment, DepthBuffer);
        FrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, ColorBuffer);
    }

    public void Dispose()
    {
        FrameBuffer.Dispose();
        DepthBuffer.Dispose();
        ColorBuffer.Dispose();
    }
}