using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;

namespace ArrhythmicBattles.Util;

public class EntityGroup : IDisposable
{
    private readonly List<Entity> entities = new List<Entity>();

    public void AddEntity(params Entity[] entity)
    {
        entities.AddRange(entity);
    }
    
    public void Update(UpdateArgs args)
    {
        entities.ForEach(entity => entity.Update(args));
    }

    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        entities.ForEach(entity => (entity as IRenderable)?.Render(renderer, layerId, matrixStack, cameraData));
    }
    
    public void Dispose()
    {
        entities.ForEach(entity => entity.Dispose());
    }
}