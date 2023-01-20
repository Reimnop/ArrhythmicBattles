using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Strategy;

public class LitVertexRenderStrategy : RenderStrategy
{
    private readonly ILighting lighting;
    private readonly ShaderProgram litShader;

    public LitVertexRenderStrategy(ILighting lighting, ShaderProgram litShader)
    {
        this.lighting = lighting;
        this.litShader = litShader;
    }
    
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        LitVertexDrawData vertexDrawData = EnsureDrawDataType<LitVertexDrawData>(drawData);
        
        glStateManager.UseProgram(litShader.Handle);
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
        
        GL.Uniform3(5, lighting.AmbientLight); 

        if (lighting.DirectionalLight.HasValue)
        {
            GL.Uniform3(6, lighting.DirectionalLight.Value.Direction);
            GL.Uniform3(7, lighting.DirectionalLight.Value.Color);
        }
        
        GL.Uniform1(8, lighting.DirectionalLight?.Intensity ?? 0.0f);

        GL.DrawElements(PrimitiveType.Triangles, vertexDrawData.Count, DrawElementsType.UnsignedInt, 0);
    }

    public override void Dispose()
    {
    }
}