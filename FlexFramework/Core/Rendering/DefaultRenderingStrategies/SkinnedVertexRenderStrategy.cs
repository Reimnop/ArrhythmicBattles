using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.DefaultRenderingStrategies;

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
        Matrix4 model = vertexDrawData.ModelMatrix;
        GL.UniformMatrix4(0, true, ref transformation);
        GL.UniformMatrix4(1, true, ref model);
        GL.Uniform1(2, vertexDrawData.Texture == null ? 0 : 1);

        if (vertexDrawData.Texture != null)
        {
            glStateManager.BindTextureUnit(0, vertexDrawData.Texture.Handle);
        }

        GL.Uniform4(4, vertexDrawData.Color);
        GL.Uniform3(5, new Vector3(0.0f, -1.1f, -1.5f));

        if (vertexDrawData.Bones != null)
        {
            for (int i = 0; i < vertexDrawData.Bones.Length; i++)
            {
                Matrix4 bone = vertexDrawData.Bones[i];
                GL.UniformMatrix4(6 + i, true, ref bone);
            }
        }

        GL.DrawElements(PrimitiveType.Triangles, vertexDrawData.Count, DrawElementsType.UnsignedInt, 0);
    }

    public override void Dispose()
    {
        skinnedShader.Dispose();
    }
}