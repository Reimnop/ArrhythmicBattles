using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core.Rendering;

public class GLStateManager
{
    private int currentFramebuffer = 0;
    private int currentProgram = 0;
    private int currentVertexArray = 0;
    private int[] currentTextureUnits = new int[16];

    private readonly Dictionary<EnableCap, bool> glCapabilities = new Dictionary<EnableCap, bool>();

    public void SetCapability(EnableCap cap, bool enabled)
    {
        if (!glCapabilities.TryGetValue(cap, out bool currentlyEnabled))
        {
            SetCapabilityInternal(cap, enabled);
            glCapabilities.Add(cap, enabled);
            return;
        }
        
        if (currentlyEnabled == enabled)
        {
            return;
        }

        glCapabilities[cap] = enabled;
        SetCapabilityInternal(cap, enabled);
    }

    private void SetCapabilityInternal(EnableCap cap, bool enabled)
    {
        if (enabled)
        {
            GL.Enable(cap);
        }
        else
        {
            GL.Disable(cap);
        }
    }

    public void BindFramebuffer(int framebuffer)
    {
        if (currentFramebuffer == framebuffer)
        {
            return;
        }

        currentFramebuffer = framebuffer;
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
    }

    public void UseProgram(int program)
    {
        if (currentProgram == program)
        {
            return;
        }
        
        currentProgram = program;
        GL.UseProgram(program);
    }
    
    public void BindVertexArray(int vertexArray)
    {
        if (currentVertexArray == vertexArray)
        {
            return;
        }
        
        currentVertexArray = vertexArray;
        GL.BindVertexArray(vertexArray);
    }
    
    public void BindTextureUnit(int unit, int texture)
    {
        if (currentTextureUnits[unit] == texture)
        {
            return;
        }
        
        currentTextureUnits[unit] = texture;
        GL.BindTextureUnit(unit, texture);
    }
}