using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering;

public class ScreenCapturer : IDisposable
{
    public int Width { get; }
    public int Height { get; }

    public Framebuffer FrameBuffer { get; }
    public Texture2D ColorBuffer { get; }
    public Renderbuffer? DepthBuffer { get; }

    public ScreenCapturer(string name, int width, int height, bool useDepth = true)
    {
        Width = width;
        Height = height;

        ColorBuffer = new Texture2D($"{name}-color", width, height, SizedInternalFormat.Rgba16f);
        ColorBuffer.SetMinFilter(TextureMinFilter.Linear);
        ColorBuffer.SetMagFilter(TextureMagFilter.Linear);
        ColorBuffer.SetWrapS(TextureWrapMode.ClampToEdge);
        ColorBuffer.SetWrapT(TextureWrapMode.ClampToEdge);

        FrameBuffer = new Framebuffer(name);
        FrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, ColorBuffer);
        
        if (useDepth)
        {
            DepthBuffer = new Renderbuffer($"{name}-depth", width, height, RenderbufferStorage.DepthComponent32f);
            FrameBuffer.Renderbuffer(FramebufferAttachment.DepthAttachment, DepthBuffer);
        }
    }

    public void Dispose()
    {
        FrameBuffer.Dispose();
        ColorBuffer.Dispose();
        DepthBuffer?.Dispose();
    }
}