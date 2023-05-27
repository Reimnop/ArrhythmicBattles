using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class LitMeshEntity : Entity, IRenderable
{
    public Mesh<LitVertex> Mesh { get; set; }
    public Vector3 Albedo { get; set; } = Vector3.One; // White
    public float Metallic { get; set; } = 0.0f;
    public float Roughness { get; set; } = 1.0f;
    public Texture? AlbedoTexture { get; set; }
    public Texture? MetallicTexture { get; set; }
    public Texture? RoughnessTexture { get; set; }
    
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
            AlbedoTexture?.ReadOnly, MetallicTexture?.ReadOnly, RoughnessTexture?.ReadOnly,
            materialData);

        commandList.AddDrawData(layerType, vertexDrawData);
    }
}