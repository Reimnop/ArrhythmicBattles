using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Sampler = FlexFramework.Core.Rendering.Data.Sampler;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class TextRenderStrategy : RenderStrategy, IDisposable
{
    private readonly ShaderProgram textShader;
    private readonly Sampler sampler;
    private readonly MeshHandler meshHandler = new(
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.TexCoord0, 1)
    );
    private readonly TextureHandler textureHandler = new();
    
    public TextRenderStrategy()
    {
        using var vertexShader = new Shader("text_vert", File.ReadAllText("Assets/Shaders/text.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("text_frag", File.ReadAllText("Assets/Shaders/text.frag"), ShaderType.FragmentShader);
        
        textShader = new ShaderProgram("text");
        textShader.LinkShaders(vertexShader, fragmentShader);
        
        sampler = new Sampler("text_sampler");
        sampler.Parameter(SamplerParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
        sampler.Parameter(SamplerParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
        sampler.Parameter(SamplerParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
        sampler.Parameter(SamplerParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
    }

    public override void Update(UpdateArgs args)
    {
        meshHandler.Update(args.DeltaTime);
    }

    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        var textDrawData = EnsureDrawDataType<TextDrawData>(drawData);
        
        var mesh = meshHandler.GetMesh(textDrawData.Mesh);
        var texture = textureHandler.GetTexture(textDrawData.FontAtlas);
        
        glStateManager.UseProgram(textShader);
        glStateManager.BindVertexArray(mesh.VertexArray);
        glStateManager.BindTextureUnit(0, texture);
        glStateManager.BindSampler(0, sampler);

        Matrix4 transformation = textDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref transformation);
        GL.Uniform4(2, textDrawData.Color);
        GL.Uniform1(3, textDrawData.DistanceRange);
        
        if (textDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(PrimitiveType.Triangles, textDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(PrimitiveType.Triangles, 0, textDrawData.Mesh.VerticesCount);
    }

    public void Dispose()
    {
        textShader.Dispose();
        sampler.Dispose();
    }
}