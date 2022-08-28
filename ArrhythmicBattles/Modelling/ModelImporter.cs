using ArrhythmicBattles.Modelling.Animate;
using ArrhythmicBattles.Util;
using Assimp;
using FlexFramework.Core.Data;
using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace ArrhythmicBattles.Modelling;

public class ModelImporter : IDisposable
{
    private readonly AssimpContext context;
    private readonly Scene scene;
    private readonly string directory;

    public ModelImporter(string path)
    {
        directory = Path.GetDirectoryName(path);
        
        context = new AssimpContext();
        scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
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
                    texture.SetMinFilter(TextureMinFilter.Nearest);
                    texture.SetMagFilter(TextureMagFilter.Nearest);
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

    public List<IndexedMesh<Vertex>> LoadMeshes()
    {
        List<IndexedMesh<Vertex>> meshes = new List<IndexedMesh<Vertex>>();

        foreach (Mesh sceneMesh in scene.Meshes)
        {
            List<Vertex> vertices = new List<Vertex>();
            for (int i = 0; i < sceneMesh.VertexCount; i++)
            {
                Vector3D pos = sceneMesh.Vertices[i];
                Vector3D uv = sceneMesh.TextureCoordinateChannelCount == 0 
                    ? new Vector3D() 
                    : sceneMesh.TextureCoordinateChannels[0][i];
                Color4D color = sceneMesh.VertexColorChannelCount == 0
                    ? new Color4D(1.0f)
                    : sceneMesh.VertexColorChannels[0][i];
                
                vertices.Add(new Vertex(
                    pos.X, pos.Y, pos.Z, 
                    uv.X, uv.Y, 
                    color.R, color.G, color.B, color.A));
            }

            IndexedMesh<Vertex> mesh = new IndexedMesh<Vertex>(sceneMesh.Name, vertices.ToArray(), sceneMesh.GetIndices());

            meshes.Add(mesh);
        }

        return meshes;
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

        Matrix4x4 aTransform = node.Transform;
        Matrix4 transform = new Matrix4(
            aTransform.A1, aTransform.B1, aTransform.C1, aTransform.D1,
            aTransform.A2, aTransform.B2, aTransform.C2, aTransform.D2,
            aTransform.A3, aTransform.B3, aTransform.C3, aTransform.D3,
            aTransform.A4, aTransform.B4, aTransform.C4, aTransform.D4);

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