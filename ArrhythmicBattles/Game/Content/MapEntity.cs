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
        var audioData = resourceManager.Get<AudioData>(mapMeta.Music);
        var stream = source.CreateStream(audioData, true);
        stream.Play();
        
        // Bind the music volume to the settings to automatically update volume when changed
        musicVolumeBinding = new Binding<float>(settings, nameof(settings.MusicVolume), source, nameof(AudioSource.Gain));
        
        // Load the props
        var propRegistry = new PropRegistry();
        foreach (var propInfo in mapMeta.Props)
        {
            var prop = propRegistry[propInfo.Identifier];
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