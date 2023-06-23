using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class RenderBufferRenderStrategy : RenderStrategy
{
    private readonly ShaderProgram program;
    private readonly MeshHandler meshHandler = new (
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.TexCoord0, 1),
            (VertexAttributeIntent.Color, 2)
        );
    private readonly TextureHandler textureHandler = new();
    private readonly SamplerHandler samplerHandler = new();

    public RenderBufferRenderStrategy()
    {
        using var vertexShader = new Shader("unlit_vert", File.ReadAllText("Assets/Shaders/unlit.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("unlit_frag", File.ReadAllText("Assets/Shaders/unlit.frag"), ShaderType.FragmentShader);
        
        program = new ShaderProgram("unlit");
        program.LinkShaders(vertexShader, fragmentShader);
    }

    public override void Update(UpdateArgs args)
    {
        meshHandler.Update(args.DeltaTime);
        textureHandler.Update(args.DeltaTime);
    }

    public override void Draw(GLStateManager glStateManager, CommandList commandList, IDrawData drawData)
    {
        var renderBufferDrawData = EnsureDrawDataType<RenderBufferDrawData>(drawData);
        
        var mesh = meshHandler.GetMesh(renderBufferDrawData.Mesh);
        var texture = renderBufferDrawData.RenderBuffer.Texture;

        glStateManager.UseProgram(program);
        glStateManager.BindVertexArray(mesh.VertexArray);

        Matrix4 transformation = renderBufferDrawData.Transformation;
        GL.UniformMatrix4(program.GetUniformLocation("mvp"), true, ref transformation);
        GL.Uniform1(program.GetUniformLocation("hasTexture"), 1);

        glStateManager.BindTextureUnit(0, texture);
        glStateManager.BindSampler(0, null);

        GL.Uniform4(program.GetUniformLocation("color"), Color4.White);

        if (renderBufferDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(renderBufferDrawData.PrimitiveType, renderBufferDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(renderBufferDrawData.PrimitiveType, 0, renderBufferDrawData.Mesh.VerticesCount);
    }
}