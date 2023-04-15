using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class TextRenderStrategy : RenderStrategy, IDisposable
{
    private readonly FlexFrameworkMain engine;
    private readonly TextAssets textAssets;
    private readonly ShaderProgram textShader;
    
    public TextRenderStrategy(FlexFrameworkMain engine)
    {
        this.engine = engine;
        var textAssetLocation = engine.DefaultAssets.TextAssets;
        textAssets = engine.ResourceRegistry.GetResource(textAssetLocation);

        using var vertexShader = new Shader("text-vert", File.ReadAllText("Assets/Shaders/text.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("text-frag", File.ReadAllText("Assets/Shaders/text.frag"), ShaderType.FragmentShader);
        
        textShader = new ShaderProgram("text");
        textShader.LinkShaders(vertexShader, fragmentShader);
    }
    
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        TextDrawData textDrawData = EnsureDrawDataType<TextDrawData>(drawData);
        
        glStateManager.UseProgram(textShader.Handle);
        glStateManager.BindVertexArray(textDrawData.VertexArray.Handle);

        Matrix4 transformation = textDrawData.Transformation;
        GL.UniformMatrix4(0, true, ref transformation);
                
        for (int i = 0; i < textAssets.AtlasTextures.Count; i++)
        {
            GL.Uniform1(i + 1, i);
            glStateManager.BindTextureUnit(i, textAssets.AtlasTextures[i].Handle);
        }

        GL.Uniform4(17, textDrawData.Color);
        GL.Uniform1(18, textDrawData.DistanceRange);
                
        GL.DrawArrays(PrimitiveType.Triangles, 0, textDrawData.Count);
    }

    public void Dispose()
    {
        textShader.Dispose();
    }
}