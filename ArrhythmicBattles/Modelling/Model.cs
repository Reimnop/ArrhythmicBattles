using ArrhythmicBattles.Modelling.Animate;
using ArrhythmicBattles.Util;
using FlexFramework.Core.Data;
using FlexFramework.Rendering.Data;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Modelling;

public class ModelMaterial : IDisposable
{
    public Color4 Color { get; }
    public Texture2D? Texture { get; }

    public ModelMaterial(Color4 color, Texture2D? texture)
    {
        Color = color;
        Texture = texture;
    }

    public void Dispose()
    {
        Texture?.Dispose();
    }
}

public class ModelMesh
{
    public int MeshIndex { get; }
    public int MaterialIndex { get; }

    public ModelMesh(int meshIndex, int materialIndex)
    {
        MeshIndex = meshIndex;
        MaterialIndex = materialIndex;
    }
}

public class ModelNode
{
    public string Name { get; }
    public Matrix4 Transform { get; }
    public IReadOnlyList<ModelMesh> Meshes => meshes;

    private readonly List<ModelMesh> meshes;

    public ModelNode(string name, Matrix4 transform, List<ModelMesh> meshes)
    {
        Name = name;
        Transform = transform;
        this.meshes = meshes;
    }
}

public class ModelBone
{
    public string Name { get; }
    public int Index { get; }
    public Matrix4 Offset { get; }

    public ModelBone(string name, int index, Matrix4 offset)
    {
        Name = name;
        Index = index;
        Offset = offset;
    }
}

public class Model : IDisposable
{
    public ImmutableTree<ModelNode> Tree { get; }
    public IReadOnlyList<IndexedMesh<Vertex>> Meshes => meshes;
    public IReadOnlyList<IndexedMesh<SkinnedVertex>> SkinnedMeshes => skinnedMeshes;
    public IReadOnlyList<ModelMaterial> Materials => materials;
    public IReadOnlyList<ModelAnimation> Animations => animations;
    public IReadOnlyList<ModelBone> Bones => bones;
    public IReadOnlyDictionary<string, int> BoneIndexMap => boneIndexMap;
    
    private readonly List<IndexedMesh<Vertex>> meshes;
    private readonly List<IndexedMesh<SkinnedVertex>> skinnedMeshes;
    private readonly List<ModelMaterial> materials;
    private readonly List<ModelAnimation> animations;
    private readonly List<ModelBone> bones;
    private readonly Dictionary<string, int> boneIndexMap;

    public Model(string path)
    {
        using ModelImporter importer = new ModelImporter(path);
        
        Tree = importer.LoadModel();
        meshes = importer.LoadMeshes();
        skinnedMeshes = importer.LoadSkinnedMeshes();
        materials = importer.LoadMaterials();
        animations = importer.LoadAnimations();
        bones = importer.LoadBones();

        boneIndexMap = importer.GetBoneIndexMap();
    }

    public void Dispose()
    {
        materials.ForEach(x => x.Dispose());
    }
}