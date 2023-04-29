using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class LitVertexRenderStrategy : RenderStrategy
{
    private readonly ILighting lighting;
    private readonly ShaderProgram litShader;
    
    private readonly MeshHandler meshHandler = new (
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.Normal, 1),
            (VertexAttributeIntent.TexCoord0, 2),
            (VertexAttributeIntent.Color, 3)
        );

    public LitVertexRenderStrategy(ILighting lighting)
    {
        this.lighting = lighting;
        
        using var vertexShader = new Shader("lit-vert", File.ReadAllText("Assets/Shaders/lit.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("lit-frag", File.ReadAllText("Assets/Shaders/lit.frag"), ShaderType.FragmentShader);
        
        litShader = new ShaderProgram("lit");
        litShader.LinkShaders(vertexShader, fragmentShader);
    }
    
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        LitVertexDrawData vertexDrawData = EnsureDrawDataType<LitVertexDrawData>(drawData);

        var (vertexArray, vertexBuffer, indexBuffer) = meshHandler.GetMesh(vertexDrawData.Mesh);
        
        glStateManager.UseProgram(litShader.Handle);
        glStateManager.BindVertexArray(vertexArray.Handle);

        Matrix4 transformation = vertexDrawData.Transformation;
        Matrix4 model = vertexDrawData.ModelMatrix;
        GL.UniformMatrix4(0, true, ref transformation);
        GL.UniformMatrix4(1, true, ref model);
        GL.Uniform1(2, vertexDrawData.Texture == null ? 0 : 1);
        
        // TODO: implement texture binding
        // if (vertexDrawData.Texture != null)
        // {
        //     glStateManager.BindTextureUnit(0, vertexDrawData.Texture.Handle);
        // }

        GL.Uniform4(4, vertexDrawData.Color);
        
        GL.Uniform3(5, lighting.AmbientLight); 

        if (lighting.DirectionalLight.HasValue)
        {
            GL.Uniform3(6, lighting.DirectionalLight.Value.Direction);
            GL.Uniform3(7, lighting.DirectionalLight.Value.Color);
        }
        
        GL.Uniform1(8, lighting.DirectionalLight?.Intensity ?? 0.0f);

        if (vertexDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(PrimitiveType.Triangles, vertexDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexDrawData.Mesh.VerticesCount);
        
        // TODO: this is dumb
        vertexArray.Dispose();
        vertexBuffer.Dispose();
        indexBuffer?.Dispose();
    }
}