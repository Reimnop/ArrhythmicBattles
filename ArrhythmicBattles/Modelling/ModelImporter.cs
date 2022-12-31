using ArrhythmicBattles.Modelling.Animate;
using ArrhythmicBattles.Util;
using Assimp;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace ArrhythmicBattles.Modelling;

public class ModelImporter : IDisposable
{
    private readonly AssimpContext context;
    private readonly Scene scene;
    private readonly string directory;

    private readonly List<ModelBone> modelBones;
    private readonly Dictionary<string, int> modelBoneIndexMap;

    public ModelImporter(string path)
    {
        directory = Path.GetDirectoryName(path);
        
        context = new AssimpContext();
        scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.FlipUVs);
        
        // Collect all bones
        Dictionary<string, ModelBone> boneNameToBone = new Dictionary<string, ModelBone>();
        int boneIndex = 0;
        
        foreach (Mesh mesh in scene.Meshes)
        {
            foreach (Bone bone in mesh.Bones)
            {
                if (boneNameToBone.ContainsKey(bone.Name))
                {
                    continue;
                }

                Matrix4x4 aiOffset = bone.OffsetMatrix;
                Matrix4 offset = new Matrix4(
                    aiOffset.A1, aiOffset.B1, aiOffset.C1, aiOffset.D1,
                    aiOffset.A2, aiOffset.B2, aiOffset.C2, aiOffset.D2,
                    aiOffset.A3, aiOffset.B3, aiOffset.C3, aiOffset.D3,
                    aiOffset.A4, aiOffset.B4, aiOffset.C4, aiOffset.D4);

                ModelBone modelBone = new ModelBone(bone.Name, boneIndex, offset);
                boneNameToBone.Add(bone.Name, modelBone);
                boneIndex++;
            }
        }

        modelBones = boneNameToBone.Values
            .OrderBy(x => x.Index)
            .ToList();

        modelBoneIndexMap = boneNameToBone
            .ToDictionary(x => x.Key, x => x.Value.Index);
    }
    
    public List<ModelMaterial> LoadMaterials()
    {
        List<ModelMaterial> materials = new List<ModelMaterial>();

        foreach (Material sceneMaterial in scene.Materials)
        {
            Texture2D? texture = null;
            if (sceneMaterial.HasTextureDiffuse)
            {
                string path = GetPath(sceneMaterial.TextureDiffuse.FilePath);
                
                if (File.Exists(path)) 
                {
                    texture = Texture2D.FromFile(Path.GetFileName(path), path);
                }
            }

            ModelMaterial material = new ModelMaterial(
                new Color4(
                    sceneMaterial.ColorDiffuse.R,
                    sceneMaterial.ColorDiffuse.G,
                    sceneMaterial.ColorDiffuse.B,
                    sceneMaterial.ColorDiffuse.A),
                texture);
            materials.Add(material);
        }

        return materials;
    }
    
    public List<IndexedMesh<LitVertex>> LoadMeshes()
    {
        List<IndexedMesh<LitVertex>> meshes = new List<IndexedMesh<LitVertex>>();

        foreach (Mesh sceneMesh in scene.Meshes)
        {
            List<LitVertex> vertices = new List<LitVertex>();
            for (int i = 0; i < sceneMesh.VertexCount; i++)
            {
                Vector3D pos = sceneMesh.Vertices[i];
                Vector3D normal = sceneMesh.Normals[i];
                Vector3D uv = sceneMesh.TextureCoordinateChannelCount == 0 
                    ? new Vector3D() 
                    : sceneMesh.TextureCoordinateChannels[0][i];
                Color4D color = sceneMesh.VertexColorChannelCount == 0
                    ? new Color4D(1.0f)
                    : sceneMesh.VertexColorChannels[0][i];
                
                vertices.Add(new LitVertex(
                    pos.X, pos.Y, pos.Z, 
                    normal.X, normal.Y, normal.Z,
                    uv.X, uv.Y, 
                    color.R, color.G, color.B, color.A));
            }

            IndexedMesh<LitVertex> mesh = new IndexedMesh<LitVertex>(sceneMesh.Name, vertices.ToArray(), sceneMesh.GetIndices());
            meshes.Add(mesh);
        }

        return meshes;
    }

    public List<IndexedMesh<SkinnedVertex>> LoadSkinnedMeshes()
    {
        List<IndexedMesh<SkinnedVertex>> meshes = new List<IndexedMesh<SkinnedVertex>>();

        foreach (Mesh sceneMesh in scene.Meshes)
        {
            List<SkinnedVertex> vertices = new List<SkinnedVertex>();
            for (int i = 0; i < sceneMesh.VertexCount; i++)
            {
                Vector3D pos = sceneMesh.Vertices[i];
                Vector3D normal = sceneMesh.Normals[i];
                Vector3D uv = sceneMesh.TextureCoordinateChannelCount == 0 
                    ? new Vector3D() 
                    : sceneMesh.TextureCoordinateChannels[0][i];
                Color4D color = sceneMesh.VertexColorChannelCount == 0
                    ? new Color4D(1.0f)
                    : sceneMesh.VertexColorChannels[0][i];
                
                vertices.Add(new SkinnedVertex(
                    pos.X, pos.Y, pos.Z, 
                    normal.X, normal.Y, normal.Z,
                    uv.X, uv.Y, 
                    color.R, color.G, color.B, color.A));
            }
            
            FillVertexWeights(vertices, sceneMesh);

            IndexedMesh<SkinnedVertex> mesh = new IndexedMesh<SkinnedVertex>(sceneMesh.Name, vertices.ToArray(), sceneMesh.GetIndices());
            meshes.Add(mesh);
        }

        return meshes;
    }

    private void FillVertexWeights(List<SkinnedVertex> vertices, Mesh mesh)
    {
        foreach (Bone bone in mesh.Bones)
        {
            ModelBone modelBone = modelBones[modelBoneIndexMap[bone.Name]];
            
            foreach (VertexWeight weight in bone.VertexWeights)
            {
                vertices[weight.VertexID] = vertices[weight.VertexID].AppendWeight(new BoneWeight(modelBone.Index, weight.Weight));
            }
        }
    }

    public List<ModelBone> LoadBones()
    {
        return modelBones;
    }

    public Dictionary<string, int> GetBoneIndexMap()
    {
        return modelBoneIndexMap;
    }

    public List<ModelAnimation> LoadAnimations()
    {
        List<ModelAnimation> animations = new List<ModelAnimation>();

        foreach (Animation animation in scene.Animations)
        {
            List<ModelNodeAnimationChannel> nodeAnimationChannels = new List<ModelNodeAnimationChannel>();
            
            foreach (NodeAnimationChannel nodeAnimationChannel in animation.NodeAnimationChannels)
            {
                List<Key<Vector3>> positionKeys = nodeAnimationChannel.PositionKeys
                    .Select(x => new Key<Vector3>((float) x.Time, new Vector3(x.Value.X, x.Value.Y, x.Value.Z)))
                    .ToList();
                
                List<Key<Vector3>> scaleKeys = nodeAnimationChannel.ScalingKeys
                    .Select(x => new Key<Vector3>((float) x.Time, new Vector3(x.Value.X, x.Value.Y, x.Value.Z)))
                    .ToList();
                
                List<Key<Quaternion>> rotationKeys = nodeAnimationChannel.RotationKeys
                    .Select(x => new Key<Quaternion>((float) x.Time, new Quaternion(x.Value.X, x.Value.Y, x.Value.Z, x.Value.W)))
                    .ToList();

                nodeAnimationChannels.Add(new ModelNodeAnimationChannel(nodeAnimationChannel.NodeName, positionKeys, scaleKeys, rotationKeys));
            }

            animations.Add(new ModelAnimation(animation.Name, (float) animation.DurationInTicks, (float) animation.TicksPerSecond, nodeAnimationChannels));
        }

        return animations;
    }
    
    public ImmutableTree<ModelNode> LoadModel()
    {
        Node rootNode = scene.RootNode;
        TreeBuilder<ModelNode> modelTreeBuilder = GetModelTreeBuilder(rootNode);
        return modelTreeBuilder.Build();
    }

    private TreeBuilder<ModelNode> GetModelTreeBuilder(Node node)
    {
        List<ModelMesh> modelMeshes = new List<ModelMesh>();
        if (node.HasMeshes)
        {
            node.MeshIndices.ForEach(meshIndex => modelMeshes.Add(GetMesh(meshIndex)));
        }

        Matrix4x4 aiTransform = node.Transform;
        Matrix4 transform = new Matrix4(
            aiTransform.A1, aiTransform.B1, aiTransform.C1, aiTransform.D1,
            aiTransform.A2, aiTransform.B2, aiTransform.C2, aiTransform.D2,
            aiTransform.A3, aiTransform.B3, aiTransform.C3, aiTransform.D3,
            aiTransform.A4, aiTransform.B4, aiTransform.C4, aiTransform.D4);

        ModelNode modelNode = new ModelNode(node.Name, transform, modelMeshes);

        TreeBuilder<ModelNode> treeBuilder = new TreeBuilder<ModelNode>(modelNode);
        foreach (Node child in node.Children)
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

    private string GetPath(string path)
    {
        if (Path.IsPathRooted(path))
        {
            return path;
        }
        
        return Path.Combine(directory, path);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}