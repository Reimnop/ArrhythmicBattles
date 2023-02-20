using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Text;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Core;

public class ResourceManager : IDisposable
{
    private readonly List<object> resources = new List<object>();

    public ResourceLocation AddResource(object resource)
    {
        resources.Add(resource);
        return new ResourceLocation(resources.Count - 1);
    }
    
    public object GetResource(ResourceLocation location)
    {
        return resources[location.Id];
    }
    
    public T GetResource<T>(ResourceLocation location)
    {
        return (T) resources[location.Id];
    }

    public void Dispose()
    {
        foreach (object resource in resources)
        {
            if (resource is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}