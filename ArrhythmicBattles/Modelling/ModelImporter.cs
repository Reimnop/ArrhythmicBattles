using System.Text.RegularExpressions;
using ArrhythmicBattles.Core;
using ArrhythmicBattles.Core.Animation;
using Assimp;
using FlexFramework.Core.Data;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace ArrhythmicBattles.Modelling;

public class ModelImporter : IDisposable
{
    public IReadOnlyList<ModelBone> Bones { get; }
    public IReadOnlyDictionary<string, int> BoneIndexMap { get; }

    private readonly AssimpContext context;
    private readonly Scene scene;
    private readonly string directory;
    
    private readonly Regex embeddedTextureRegex = new(@"\*(\d+)", RegexOptions.Compiled);

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

                var modelBone = new ModelBone(bone.Name, boneIndex, offset);
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
            Texture? albedoTexture = null;
            if (material.HasTextureDiffuse)
            {
                albedoTexture = LoadTexture(material.TextureDiffuse);
            }

            var albedo = new Vector3(material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B);
            var metallic = material.Reflectivity;
            var roughness = 1.0f - material.Shininess;

            return new ModelMaterial(material.Name, albedo, metallic, roughness, albedoTexture, null, null);
        });
    }

    private Texture? LoadTexture(TextureSlot textureSlot)
    {
        // Check if texture is embedded
        var match = embeddedTextureRegex.Match(textureSlot.FilePath);
        if (match.Success)
        {
            var index = int.Parse(match.Groups[1].Value);
            var embeddedTexture = scene.Textures[index];

            // Check if texture is compressed
            if (embeddedTexture.IsCompressed)
            {
                var data = embeddedTexture.CompressedData;
                // Use ImageSharp to decompress
                var image = Image.LoadPixelData<Rgba32>(data, embeddedTexture.Width, embeddedTexture.Height);
                var pixels = new byte[image.Width * image.Height * 4]; // 4 bytes per pixel
                image.CopyPixelDataTo(pixels);
                
                return new Texture($"embedded {index}", image.Width, image.Height, PixelFormat.Rgba8, pixels);
            }
            
            // Not compressed, use raw data
            byte[] rawData = new byte[embeddedTexture.Width * embeddedTexture.Height * 4];
            
            // Transpose the data
            for (int i = 0; i < embeddedTexture.NonCompressedData.Length; i++)
            {
                rawData[i * 4 + 0] = embeddedTexture.NonCompressedData[i].R;
                rawData[i * 4 + 1] = embeddedTexture.NonCompressedData[i].G;
                rawData[i * 4 + 2] = embeddedTexture.NonCompressedData[i].B;
                rawData[i * 4 + 3] = embeddedTexture.NonCompressedData[i].A;
            }
            
            // Create texture
            return new Texture($"embedded {index}", embeddedTexture.Width, embeddedTexture.Height, PixelFormat.Rgba8, rawData);
        }
        
        // Not embedded, load from file
        var path = Path.GetFullPath(textureSlot.FilePath, directory);

        if (File.Exists(path)) 
        {
            return Texture.FromFile(Path.GetFileName(path), path);
        }

        return null;
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