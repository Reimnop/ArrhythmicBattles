using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Rendering;

public class GLStateManager
{
    private int currentProgram = 0;
    private int currentVertexArray = 0;
    private int[] currentTextureUnits = new int[16];

    private readonly Dictionary<EnableCap, bool> glCapabilities = new Dictionary<EnableCap, bool>();

    public void SetCapability(EnableCap cap, bool enabled)
    {
        if (!glCapabilities.TryGetValue(cap, out bool currentlyEnabled))
        {
            SetCapabilityIgnoreChecks(cap, enabled);
            glCapabilities.Add(cap, enabled);
            return;
        }
        
        if (currentlyEnabled == enabled)
        {
            return;
        }

        glCapabilities[cap] = enabled;
        SetCapabilityIgnoreChecks(cap, enabled);
    }

    private void SetCapabilityIgnoreChecks(EnableCap cap, bool enabled)
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