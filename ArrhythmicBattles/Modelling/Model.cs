using ArrhythmicBattles.Modelling.Animate;
using ArrhythmicBattles.Util;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
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
    private readonly ModelImporter modelImporter;
    
    public ImmutableTree<ModelNode> Tree { get; }

    // lazily load meshes
    // yes, Rider converted my if statements and getter to this
    public IReadOnlyList<IndexedMesh<LitVertex>> Meshes => meshes ??= modelImporter.LoadMeshes();
    public IReadOnlyList<IndexedMesh<SkinnedVertex>> SkinnedMeshes => skinnedMeshes ??= modelImporter.LoadSkinnedMeshes();

    public IReadOnlyList<ModelMaterial> Materials => materials;
    public IReadOnlyList<ModelAnimation> Animations => animations;
    public IReadOnlyList<ModelBone> Bones => bones;
    public IReadOnlyDictionary<string, int> BoneIndexMap => boneIndexMap;
    
    private List<IndexedMesh<LitVertex>>? meshes;
    private List<IndexedMesh<SkinnedVertex>>? skinnedMeshes;
    
    private readonly List<ModelMaterial> materials;
    private readonly List<ModelAnimation> animations;
    private readonly List<ModelBone> bones;
    private readonly Dictionary<string, int> boneIndexMap;

    public Model(string path)
    {
        modelImporter = new ModelImporter(path);
        
        Tree = modelImporter.LoadModel();
        meshes = modelImporter.LoadMeshes();
        skinnedMeshes = modelImporter.LoadSkinnedMeshes();
        materials = modelImporter.LoadMaterials();
        animations = modelImporter.LoadAnimations();
        bones = modelImporter.LoadBones();

        boneIndexMap = modelImporter.GetBoneIndexMap();
    }

    public void TextureMinFilter(TextureMinFilter filter)
    {
        materials.ForEach(mat => mat.Texture?.SetMinFilter(filter));
    }
    
    public void TextureMagFilter(TextureMagFilter filter)
    {
        materials.ForEach(mat => mat.Texture?.SetMagFilter(filter));
    }
    
    public void TextureAnisotropicFiltering(float value)
    {
        materials.ForEach(mat => mat.Texture?.AnisotropicFiltering(value));
    }

    public void TextureGenerateMipmap()
    {
        materials.ForEach(mat => mat.Texture?.GenerateMipmap());
    }

    public void Dispose()
    {
        materials.ForEach(x => x.Dispose());
        modelImporter.Dispose();
    }
}