using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class SkinnedVertexRenderStrategy : RenderStrategy, IDisposable
{
    private readonly ILighting lighting;
    private readonly ShaderProgram skinnedShader;
    
    private readonly MeshHandler meshHandler = new(
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.Normal, 1),
            (VertexAttributeIntent.TexCoord0, 2),
            (VertexAttributeIntent.Color, 3),
            (VertexAttributeIntent.BoneIndex, 4),
            (VertexAttributeIntent.BoneWeight, 5)
        );
    
    private readonly TextureHandler textureHandler = new();
    
    public SkinnedVertexRenderStrategy(ILighting lighting)
    {
        this.lighting = lighting;
        
        using var vertexShader = new Shader("skinned-vs", File.ReadAllText("Assets/Shaders/skinned.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("skinned-fs", File.ReadAllText("Assets/Shaders/lit.frag"), ShaderType.FragmentShader);

        skinnedShader = new ShaderProgram("skinned");
        skinnedShader.LinkShaders(vertexShader, fragmentShader);
    }
    
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        SkinnedVertexDrawData vertexDrawData = EnsureDrawDataType<SkinnedVertexDrawData>(drawData);
        
        var (vertexArray, vertexBuffer, indexBuffer) = meshHandler.GetMesh(vertexDrawData.Mesh);
        Texture2D? texture = vertexDrawData.Texture != null ? textureHandler.GetTexture(vertexDrawData.Texture) : null;
        
        glStateManager.UseProgram(skinnedShader);
        glStateManager.BindVertexArray(vertexArray);

        Matrix4 transformation = vertexDrawData.Transformation;
        Matrix4 model = vertexDrawData.ModelMatrix;
        GL.UniformMatrix4(0, true, ref transformation);
        GL.UniformMatrix4(1, true, ref model);
        GL.Uniform1(2, vertexDrawData.Texture == null ? 0 : 1);

        if (texture != null)
        {
            glStateManager.BindTextureUnit(0, texture);
        }

        GL.Uniform4(4, vertexDrawData.Color);
        
        GL.Uniform3(5, lighting.AmbientLight); 

        if (lighting.DirectionalLight.HasValue)
        {
            GL.Uniform3(6, lighting.DirectionalLight.Value.Direction);
            GL.Uniform3(7, lighting.DirectionalLight.Value.Color);
        }
        
        GL.Uniform1(8, lighting.DirectionalLight?.Intensity ?? 0.0f);

        if (vertexDrawData.Bones != null)
        {
            for (int i = 0; i < vertexDrawData.Bones.Length; i++)
            {
                Matrix4 bone = vertexDrawData.Bones[i];
                GL.UniformMatrix4(9 + i, true, ref bone);
            }
        }

        if (vertexDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(PrimitiveType.Triangles, vertexDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexDrawData.Mesh.VerticesCount);
        
        vertexArray.Dispose();
        vertexBuffer.Dispose();
        indexBuffer?.Dispose();
        texture?.Dispose();
    }

    public void Dispose()
    {
        skinnedShader.Dispose();
    }
}