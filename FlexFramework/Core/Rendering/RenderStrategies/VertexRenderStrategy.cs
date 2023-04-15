using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class VertexRenderStrategy : RenderStrategy
{
    private readonly ShaderProgram unlitShader;

    public VertexRenderStrategy()
    {
        using var vertexShader = new Shader("unlit-vert", File.ReadAllText("Assets/Shaders/unlit.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("unlit-frag", File.ReadAllText("Assets/Shaders/unlit.frag"), ShaderType.FragmentShader);
        
        unlitShader = new ShaderProgram("unlit");
        unlitShader.LinkShaders(vertexShader, fragmentShader);
    }
    
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        VertexDrawData vertexDrawData = EnsureDrawDataType<VertexDrawData>(drawData);
        
        glStateManager.UseProgram(unlitShader.Handle);
        glStateManager.BindVertexArray(vertexDrawData.VertexArray.Handle);

        Matrix4 transformation = vertexDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref transformation);
        GL.Uniform1(1, vertexDrawData.Texture == null ? 0 : 1);

        if (vertexDrawData.Texture != null)
        {
            glStateManager.BindTextureUnit(0, vertexDrawData.Texture.Handle);
        }

        GL.Uniform4(3, vertexDrawData.Color);

        GL.DrawArrays(vertexDrawData.PrimitiveType, 0, vertexDrawData.Count);
    }
}