using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Rendering.DefaultRenderingStrategies;

public class SkinnedVertexRenderStrategy : RenderingStrategy
{
    private readonly ShaderProgram skinnedShader;
    
    public SkinnedVertexRenderStrategy()
    {
        using Shader vertexShader = new Shader("skinned-vs", File.ReadAllText("Assets/Shaders/skinned.vert"), ShaderType.VertexShader);
        using Shader fragmentShader = new Shader("skinned-fs", File.ReadAllText("Assets/Shaders/skinned.frag"), ShaderType.FragmentShader);

        skinnedShader = new ShaderProgram("skinned");
        skinnedShader.LinkShaders(vertexShader, fragmentShader);
    }
    
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        SkinnedVertexDrawData vertexDrawData = EnsureDrawDataType<SkinnedVertexDrawData>(drawData);
        
        glStateManager.UseProgram(skinnedShader.Handle);
        glStateManager.BindVertexArray(vertexDrawData.VertexArray.Handle);

        Matrix4 transformation = vertexDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref transformation);
        GL.Uniform1(1, vertexDrawData.Texture == null ? 0 : 1);

        if (vertexDrawData.Texture != null)
        {
            glStateManager.BindTextureUnit(0, vertexDrawData.Texture.Handle);
        }

        GL.Uniform4(3, vertexDrawData.Color);
        GL.Uniform1(4, vertexDrawData.Bones == null || vertexDrawData.Bones.Length == 0 ? 0 : 1);

        if (vertexDrawData.Bones != null)
        {
            for (int i = 0; i < vertexDrawData.Bones.Length; i++)
            {
                Matrix4 bone = vertexDrawData.Bones[i];
                GL.UniformMatrix4(5 + i, true, ref bone);
            }
        }

        GL.DrawElements(PrimitiveType.Triangles, vertexDrawData.Count, DrawElementsType.UnsignedInt, 0);
    }

    public override void Dispose()
    {
        skinnedShader.Dispose();
    }
}