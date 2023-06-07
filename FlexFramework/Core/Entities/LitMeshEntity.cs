using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;
using TextureSampler = FlexFramework.Core.Data.TextureSampler;

namespace FlexFramework.Core.Entities;

public class LitMeshEntity : Entity, IRenderable
{
    public Mesh<LitVertex> Mesh { get; set; }
    public Vector3 Albedo { get; set; } = Vector3.One; // White
    public float Metallic { get; set; } = 0.0f;
    public float Roughness { get; set; } = 1.0f;
    public TextureSampler? AlbedoTexture { get; set; }
    public TextureSampler? MetallicTexture { get; set; }
    public TextureSampler? RoughnessTexture { get; set; }
    
    public LitMeshEntity(Mesh<LitVertex> mesh)
    {
        Mesh = mesh;
    }

    public void Render(RenderArgs args)
    {
        var commandList = args.CommandList;
        var layerType = args.LayerType;
        var matrixStack = args.MatrixStack;
        var cameraData = args.CameraData;

        var materialData = new MaterialData
        {
            UseAlbedoTexture = AlbedoTexture != null,
            UseMetallicTexture = MetallicTexture != null,
            UseRoughnessTexture = RoughnessTexture != null,
            Albedo = Albedo,
            Metallic = Metallic,
            Roughness = Roughness
        };
        
        var vertexDrawData = new LitVertexDrawData(
            Mesh.ReadOnly, 
            matrixStack.GlobalTransformation, cameraData, 
            AlbedoTexture != null 
                ? new FlexFramework.Core.Rendering.Data.TextureSampler(
                    AlbedoTexture.Texture.ReadOnly, 
                    AlbedoTexture.Sampler.ReadOnly) 
                : null,
            MetallicTexture != null 
                ? new FlexFramework.Core.Rendering.Data.TextureSampler(
                    MetallicTexture.Texture.ReadOnly, 
                    MetallicTexture.Sampler.ReadOnly) 
                : null,
            RoughnessTexture != null
                ? new FlexFramework.Core.Rendering.Data.TextureSampler(
                    RoughnessTexture.Texture.ReadOnly, 
                    RoughnessTexture.Sampler.ReadOnly) 
                : null,
            materialData);

        commandList.AddDrawData(layerType, vertexDrawData);
    }
}