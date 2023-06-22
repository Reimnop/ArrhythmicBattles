using ArrhythmicBattles.Core;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using ArrhythmicBattles.Settings;
using ArrhythmicBattles.Util;
using FlexFramework.Core;
using FlexFramework.Core.Audio;
using FlexFramework.Core.Entities;

namespace ArrhythmicBattles.Game.Content;

// Contains props and soundtrack
public class MapEntity : Entity, IUpdateable, IRenderable, IDisposable
{
    private readonly AudioSource source;
    private readonly Binding<float> musicVolumeBinding;
    
    private readonly List<PropInstance> propInstances = new();

    public MapEntity(ResourceManager resourceManager, MapMeta mapMeta, PhysicsWorld physicsWorld, ISettings settings)
    {
        // Initialize audio
        source = new AudioSource();
        source.AudioStream = resourceManager.Get<AudioStream>(mapMeta.Music);
        source.Play();
        
        // Bind the music volume to the settings to automatically update volume when changed
        musicVolumeBinding = new Binding<float>(settings, nameof(settings.MusicVolume), source, nameof(AudioSource.Gain));
        
        // Load the props
        var propContent = new PropContent();
        foreach (var propInfo in mapMeta.Props)
        {
            var location = propContent.Registry[propInfo.Identifier];
            var prop = propContent.Registry[location];
            var propInstance = prop.CreateInstance(resourceManager, physicsWorld, propInfo.Position, propInfo.Scale, propInfo.Rotation);
            propInstances.Add(propInstance);
        }
    }

    public void Update(UpdateArgs args)
    {
        foreach (var updateable in propInstances.OfType<IUpdateable>())
        {
            updateable.Update(args);
        }
    }

    public void Render(RenderArgs args)
    {
        foreach (var renderable in propInstances.OfType<IRenderable>())
        {
            renderable.Render(args);
        }
    }

    public void Dispose()
    {
        source.Dispose();
        musicVolumeBinding.Dispose();

        foreach (var disposable in propInstances.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}