using FlexFramework.Core.Data;

namespace FlexFramework.Core;

public class EngineAssets
{
    public ResourceLocation<Mesh<Vertex>> QuadMesh { get; }
    public ResourceLocation<Mesh<Vertex>> QuadWireframeMesh { get; }

    private readonly FlexFrameworkMain engine;
    private readonly ResourceRegistry registry;
    
    public EngineAssets(FlexFrameworkMain engine, ResourceRegistry registry)
    {
        this.engine = engine;
        this.registry = registry;
        
        QuadMesh = CreateQuadMesh();
        QuadWireframeMesh = CreateQuadWireframeMesh();
    }

    private ResourceLocation<Mesh<Vertex>> CreateQuadWireframeMesh()
    {
        Vertex[] vertices =
        {
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, -0.5f, 0.0f, 1.0f, 0.0f)
        };
        
        return registry.Register(new Mesh<Vertex>("quad-wireframe", vertices));
    }
    
    private ResourceLocation<Mesh<Vertex>> CreateQuadMesh()
    {
        Vertex[] vertices =
        {
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, -0.5f, 0.0f, 1.0f, 0.0f)
        };
        
        return registry.Register(new Mesh<Vertex>("quad", vertices));
    }
}