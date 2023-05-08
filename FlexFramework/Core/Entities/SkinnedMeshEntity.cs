using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class SkinnedMeshEntity : Entity, IRenderable
{
    public Mesh<SkinnedVertex>? Mesh { get; set; }
    public Texture? AlbedoTexture { get; set; }
    public Texture? MetallicTexture { get; set; }
    public Texture? RoughnessTexture { get; set; }
    public Vector3 Albedo { get; set; } = Vector3.One; // White
    public float Metallic { get; set; } = 0.0f;
    public float Roughness { get; set; } = 1.0f;

    // A single set of bones can be used for multiple meshes,
    // Hence this is readonly
    private readonly Matrix4[] bones;

    public SkinnedMeshEntity(Matrix4[] bones)
    {
        this.bones = bones;
    }

    public void Render(RenderArgs args)
    {
        if (Mesh == null)
        {
            return;
        }
        
        Renderer renderer = args.Renderer;
        int layerId = args.LayerId;
        MatrixStack matrixStack = args.MatrixStack;
        CameraData cameraData = args.CameraData;
        
        MaterialData materialData = new MaterialData()
        {
            UseAlbedoTexture = AlbedoTexture != null,
            UseMetallicTexture = MetallicTexture != null,
            UseRoughnessTexture = RoughnessTexture != null,
            Albedo = Albedo,
            Metallic = Metallic,
            Roughness = Roughness
        };
        
        SkinnedVertexDrawData vertexDrawData = new SkinnedVertexDrawData(
            Mesh.ReadOnly, 
            matrixStack.GlobalTransformation, cameraData,
            bones,
            AlbedoTexture?.ReadOnly, MetallicTexture?.ReadOnly, RoughnessTexture?.ReadOnly,
            materialData);
        
        renderer.EnqueueDrawData(layerId, vertexDrawData);
    }
}