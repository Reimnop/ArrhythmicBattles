using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Rendering.Data;

public class Framebuffer : IGpuObject
{
    public int Handle { get; }
    public string Name { get; }

    public Framebuffer(string name)
    {
        Name = name;
        
        GL.CreateFramebuffers(1, out int handle);
        GL.ObjectLabel(ObjectLabelIdentifier.Framebuffer, handle, name.Length, name);

        Handle = handle;
    }

    public void Renderbuffer(FramebufferAttachment attachment, Renderbuffer renderbuffer)
    {
        GL.NamedFramebufferRenderbuffer(Handle, attachment, RenderbufferTarget.Renderbuffer, renderbuffer.Handle);
    }
    
    public void Texture(FramebufferAttachment attachment, Texture2D texture2D, int level = 0)
    {
        GL.NamedFramebufferTexture(Handle, attachment, texture2D.Handle, level);
    }

    public void DrawBuffer(DrawBufferMode drawBufferMode)
    {
        GL.NamedFramebufferDrawBuffer(Handle, drawBufferMode);
    }
    
    public void ReadBuffer(ReadBufferMode readBufferMode)
    {
        GL.NamedFramebufferReadBuffer(Handle, readBufferMode);
    }
    
    public void Dispose()
    {
        GL.DeleteFramebuffer(Handle);
    }
}