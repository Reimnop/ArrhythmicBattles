using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering.Data;

public class Renderbuffer : GpuObject
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

    public override void Dispose()
    {
        GL.DeleteRenderbuffer(Handle);
    }
}