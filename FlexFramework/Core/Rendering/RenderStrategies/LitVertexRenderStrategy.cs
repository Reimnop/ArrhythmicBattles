﻿using OpenTK.Mathematics;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using Buffer = FlexFramework.Core.Rendering.Data.Buffer;
using FlexFramework.Core.Rendering.Lighting;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class LitVertexRenderStrategy : RenderStrategy
{
    private readonly ShaderProgram program;
    
    private readonly MeshHandler meshHandler = new(
            (VertexAttributeIntent.Position, 0),
            (VertexAttributeIntent.Normal, 1),
            (VertexAttributeIntent.TexCoord0, 2),
            (VertexAttributeIntent.Color, 3)
        );
    private readonly TextureHandler textureHandler = new();
    private readonly SamplerHandler samplerHandler = new();
    private readonly Buffer materialBuffer;

    public LitVertexRenderStrategy()
    {
        // Shader init
        using var vertexShader = new Shader("lit_vert", File.ReadAllText("Assets/Shaders/lit.vert"), ShaderType.VertexShader);
        using var fragmentShader = new Shader("lit_frag", File.ReadAllText("Assets/Shaders/lit.frag"), ShaderType.FragmentShader);
        
        program = new ShaderProgram("lit");
        program.LinkShaders(vertexShader, fragmentShader);
        
        // Buffer init
        materialBuffer = new Buffer("material");
    }

    public override void Update(UpdateArgs args)
    {
        meshHandler.Update(args.DeltaTime);
        textureHandler.Update(args.DeltaTime);
    }

    public override void Draw(GLStateManager glStateManager, CommandList commandList, IDrawData drawData)
    {
        var vertexDrawData = EnsureDrawDataType<LitVertexDrawData>(drawData);
        var material = vertexDrawData.Material;
        
        materialBuffer.LoadData(material);

        var mesh = meshHandler.GetMesh(vertexDrawData.Mesh);
        var albedo = vertexDrawData.Albedo;
        var metallic = vertexDrawData.Metallic;
        var roughness = vertexDrawData.Roughness;

        glStateManager.UseProgram(program);
        glStateManager.BindVertexArray(mesh.VertexArray);
        
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, materialBuffer.Handle);

        var mvp = vertexDrawData.Transformation * vertexDrawData.Camera.View * vertexDrawData.Camera.Projection;
        var model = vertexDrawData.Transformation;
        GL.UniformMatrix4(program.GetUniformLocation("mvp"), true, ref mvp);
        GL.UniformMatrix4(program.GetUniformLocation("model"), true, ref model);

        if (albedo.HasValue)
        {
            var texture = textureHandler.GetTexture(albedo.Value.Texture);
            var sampler = samplerHandler.GetSampler(albedo.Value.Sampler);
            
            GL.Uniform1(program.GetUniformLocation("albedoTex"), 0);
            glStateManager.BindTextureUnit(0, texture);
            glStateManager.BindSampler(0, sampler);
        }
        
        if (metallic.HasValue)
        {
            var texture = textureHandler.GetTexture(metallic.Value.Texture);
            var sampler = samplerHandler.GetSampler(metallic.Value.Sampler);
            
            GL.Uniform1(program.GetUniformLocation("metallicTex"), 1);
            glStateManager.BindTextureUnit(1, texture);
            glStateManager.BindSampler(1, sampler);
        }

        if (roughness.HasValue)
        {
            var texture = textureHandler.GetTexture(roughness.Value.Texture);
            var sampler = samplerHandler.GetSampler(roughness.Value.Sampler);
            
            GL.Uniform1(program.GetUniformLocation("roughnessTex"), 2);
            glStateManager.BindTextureUnit(2, texture);
            glStateManager.BindSampler(2, sampler);
        }

        // Lighting
        commandList.TryGetLighting(out var lighting);
        var ambient = lighting?.GetAmbientLight() ?? Vector3.Zero;
        var directional = lighting?.GetDirectionalLight() ?? DirectionalLight.None;
        
        GL.Uniform3(program.GetUniformLocation("ambientColor"), ambient); 
        GL.Uniform3(program.GetUniformLocation("cameraPos"), vertexDrawData.Camera.Position);
        
        // Directional light
        GL.Uniform3(program.GetUniformLocation("lightDirection"), directional.Direction);
        GL.Uniform3(program.GetUniformLocation("lightColor"), directional.Color * directional.Intensity);
        
        // Point lights
        const int maxPointLights = 16;
        
        var pointLights = lighting?.GetPointLights() ?? Enumerable.Empty<PointLight>();
        var pointLightsCount = Math.Min(lighting?.GetPointLightsCount() ?? 0, maxPointLights);
        
        GL.Uniform1(program.GetUniformLocation("pointLightsCount"), pointLightsCount);
        var offset = 0;
        var pointLightPositionsLocation = program.GetUniformLocation("pointLightPositions");
        var pointLightColorsLocation = program.GetUniformLocation("pointLightColors");
        foreach (var pointLight in pointLights)
        {
            if (offset >= maxPointLights)
                break;
            
            GL.Uniform3(pointLightPositionsLocation + offset, pointLight.Position);
            GL.Uniform3(pointLightColorsLocation + offset, pointLight.Color * pointLight.Intensity);
            offset++;
        }

        if (vertexDrawData.Mesh.IndicesCount > 0)
            GL.DrawElements(PrimitiveType.Triangles, vertexDrawData.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        else
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexDrawData.Mesh.VerticesCount);
    }
}