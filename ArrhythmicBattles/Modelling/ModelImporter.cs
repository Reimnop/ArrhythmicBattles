using ArrhythmicBattles.Core;
using ArrhythmicBattles.Core.Animation;
using Assimp;
using FlexFramework.Core.Data;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace ArrhythmicBattles.Modelling;

public class ModelImporter : IDisposable
{
    public IReadOnlyList<ModelBone> Bones { get; }
    public IReadOnlyDictionary<string, int> BoneIndexMap { get; }

    private readonly AssimpContext context;
    private readonly Scene scene;
    private readonly string directory;

    public ModelImporter(string path)
    {
        path = Path.GetFullPath(path);
        directory = Path.GetDirectoryName(path)!;
        
        context = new AssimpContext();
        scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.FlipUVs);
        
        // Collect all bones
        var boneNameToBone = new Dictionary<string, ModelBone>();
        var boneIndex = 0;
        
        foreach (var mesh in scene.Meshes)
        {
            foreach (var bone in mesh.Bones)
            {
                if (boneNameToBone.ContainsKey(bone.Name))
                {
                    continue;
                }

                var aiOffset = bone.OffsetMatrix;
                var offset = new Matrix4(
                    aiOffset.A1, aiOffset.B1, aiOffset.C1, aiOffset.D1,
                    aiOffset.A2, aiOffset.B2, aiOffset.C2, aiOffset.D2,
                    aiOffset.A3, aiOffset.B3, aiOffset.C3, aiOffset.D3,
                    aiOffset.A4, aiOffset.B4, aiOffset.C4, aiOffset.D4);

                var inverseBindPose = Matrix4.Invert(offset);

                var modelBone = new ModelBone(bone.Name, boneIndex, offset, inverseBindPose);
                boneNameToBone.Add(bone.Name, modelBone);
                boneIndex++;
            }
        }

        Bones = boneNameToBone.Values
            .OrderBy(x => x.Index)
            .ToList();

        BoneIndexMap = boneNameToBone
            .ToDictionary(x => x.Key, x => x.Value.Index);
    }
    
    public IEnumerable<ModelMaterial> LoadMaterials()
    {
        return scene.Materials.Select(material =>
        {
            Texture? texture = null;
            if (material.HasTextureDiffuse)
            {
                var path = Path.GetFullPath(material.TextureDiffuse.FilePath, directory);

                if (File.Exists(path)) 
                {
                    texture = Texture.FromFile(Path.GetFileName(path), path);
                }
            }

            var color = new Color4(
                material.ColorDiffuse.R,
                material.ColorDiffuse.G,
                material.ColorDiffuse.B,
                material.ColorDiffuse.A);

            return new ModelMaterial(material.Name, color, 1.0f, texture);
        });
    }
    
    public IEnumerable<Mesh<LitVertex>> LoadMeshes()
    {
        return scene.Meshes.Select(mesh =>
        {
            var vertices = new List<LitVertex>();
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var pos = mesh.Vertices[i];
                var normal = mesh.Normals[i];
                var uv = mesh.TextureCoordinateChannelCount == 0 
                    ? new Vector3D() 
                    : mesh.TextureCoordinateChannels[0][i];
                var color = mesh.VertexColorChannelCount == 0
                    ? new Color4D(1.0f)
                    : mesh.VertexColorChannels[0][i];
                
                vertices.Add(new(
                    pos.X, pos.Y, pos.Z, 
                    normal.X, normal.Y, normal.Z,
                    uv.X, uv.Y, 
                    color.R, color.G, color.B, color.A));
            }

            return new Mesh<LitVertex>(mesh.Name, vertices.ToArray(), mesh.GetIndices());
        });
    }

    public IEnumerable<Mesh<SkinnedVertex>> LoadSkinnedMeshes()
    {
        return scene.Meshes.Select(mesh =>
        {
            var vertices = new List<SkinnedVertex>();
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var pos = mesh.Vertices[i];
                var normal = mesh.Normals[i];
                var uv = mesh.TextureCoordinateChannelCount == 0 
                    ? new Vector3D() 
                    : mesh.TextureCoordinateChannels[0][i];
                var color = mesh.VertexColorChannelCount == 0
                    ? new Color4D(1.0f)
                    : mesh.VertexColorChannels[0][i];
                
                vertices.Add(new(
                    pos.X, pos.Y, pos.Z, 
                    normal.X, normal.Y, normal.Z,
                    uv.X, uv.Y, 
                    color.R, color.G, color.B, color.A));
            }
            
            FillVertexWeights(vertices, mesh);

            return new Mesh<SkinnedVertex>(mesh.Name, vertices.ToArray(), mesh.GetIndices());
        });
    }

    private void FillVertexWeights(List<SkinnedVertex> vertices, Mesh mesh)
    {
        foreach (var bone in mesh.Bones)
        {
            var modelBone = Bones[BoneIndexMap[bone.Name]];
            
            foreach (VertexWeight weight in bone.VertexWeights)
            {
                vertices[weight.VertexID] = vertices[weight.VertexID].AppendWeight(new BoneWeight(modelBone.Index, weight.Weight));
            }
        }
    }

    public IEnumerable<ModelAnimation> LoadAnimations()
    {
        return scene.Animations.Select(animation => 
            new ModelAnimation(
                animation.Name,
                (float) animation.DurationInTicks,
                (float) animation.TicksPerSecond,
                animation.NodeAnimationChannels.Select(channel =>
                    new ModelNodeAnimationChannel(
                        channel.NodeName,
                        channel.PositionKeys.Select(x => new Key<Vector3>((float) x.Time, new Vector3(x.Value.X, x.Value.Y, x.Value.Z))), 
                        channel.ScalingKeys.Select(x => new Key<Vector3>((float) x.Time, new Vector3(x.Value.X, x.Value.Y, x.Value.Z))),
                        channel.RotationKeys.Select(x => new Key<Quaternion>((float) x.Time, new Quaternion(x.Value.X, x.Value.Y, x.Value.Z, x.Value.W)))
                    )
                )
            )
        );
    }

    public ImmutableNode<ModelNode> LoadModel()
    {
        var rootNode = scene.RootNode;
        var modelTreeBuilder = GetModelTreeBuilder(rootNode);
        return modelTreeBuilder.Build();
    }

    private TreeBuilder<ModelNode> GetModelTreeBuilder(Node node)
    {
        var modelMeshes = new List<ModelMesh>();
        if (node.HasMeshes)
        {
            node.MeshIndices.ForEach(meshIndex => modelMeshes.Add(GetMesh(meshIndex)));
        }

        var aiTransform = node.Transform;
        var transform = new Matrix4(
            aiTransform.A1, aiTransform.B1, aiTransform.C1, aiTransform.D1,
            aiTransform.A2, aiTransform.B2, aiTransform.C2, aiTransform.D2,
            aiTransform.A3, aiTransform.B3, aiTransform.C3, aiTransform.D3,
            aiTransform.A4, aiTransform.B4, aiTransform.C4, aiTransform.D4);

        var modelNode = new ModelNode(node.Name, transform, modelMeshes);

        var treeBuilder = new TreeBuilder<ModelNode>(modelNode);
        foreach (var child in node.Children)
        {
            treeBuilder.PushChild(GetModelTreeBuilder(child));
        }

        return treeBuilder;
    }

    private ModelMesh GetMesh(int meshIndex)
    {
        Mesh mesh = scene.Meshes[meshIndex];
        return new ModelMesh(meshIndex, mesh.MaterialIndex);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}