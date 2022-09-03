using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Rendering.Data;

public class Renderbuffer : IGpuObject
{
    public int Handle { get; }
    public string Name { get; }

    public Renderbuffer(string name, int width, int height, RenderbufferStorage format)
    {
        Name = name;
        
        GL.CreateRenderbuffers(1, out int handle);
        GL.NamedRenderbufferStorage(handle, format, width, height);
        
        GL.ObjectLabel(ObjectLabelIdentifier.Renderbuffer, handle, name.Length, name);

        Handle = handle;
    }

    public void Dispose()
    {
        GL.DeleteRenderbuffer(Handle);
    }
}