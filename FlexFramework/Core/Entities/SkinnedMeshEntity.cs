using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Entities;

public class SkinnedMeshEntity : Entity, IRenderable
{
    public Mesh<SkinnedVertex>? Mesh { get; set; }
    public Texture? Texture { get; set; }
    public Color4 Color { get; set; } = Color4.White;

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

        Matrix4 transformation = matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection;
        SkinnedVertexDrawData vertexDrawData = new SkinnedVertexDrawData(Mesh.ReadOnly, matrixStack.GlobalTransformation, transformation, Texture?.ReadOnly, bones, Color);
        
        renderer.EnqueueDrawData(layerId, vertexDrawData);
    }
}