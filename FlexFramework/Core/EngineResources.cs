using FlexFramework.Core;
using FlexFramework.Core.Data;

namespace FlexFramework.Core;

public class EngineResources
{
    public ResourceLocation QuadMesh { get; }

    private readonly ResourceManager resourceManager;
    
    public EngineResources(ResourceManager resourceManager)
    {
        this.resourceManager = resourceManager;
        QuadMesh = CreateQuadMesh();
    }
    
    private ResourceLocation CreateQuadMesh()
    {
        Vertex[] quadVertices =
        {
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, 0.5f, 0.0f, 1.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f),
            new Vertex(0.5f, -0.5f, 0.0f, 1.0f, 0.0f)
        };
        
        return resourceManager.AddResource(new Mesh<Vertex>("quad", quadVertices));
    }
}