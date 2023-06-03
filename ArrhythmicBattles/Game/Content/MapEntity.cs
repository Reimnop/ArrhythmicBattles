using ArrhythmicBattles.Core;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Settings;
using ArrhythmicBattles.Util;
using FlexFramework.Core;
using FlexFramework.Core.Audio;
using FlexFramework.Core.Entities;
using Newtonsoft.Json.Linq;

namespace ArrhythmicBattles.Game.Content;

// Contains props and soundtrack
public class MapEntity : Entity, IRenderable, IDisposable
{
    public MapMeta Meta { get; }
    
    private readonly AudioSource source;
    private readonly ContentLoader contentLoader;
    private readonly ContentLoader propContentLoader;
    
    private readonly AudioStream music;
    private readonly List<Prop> props = new();

    private readonly Binding<float> musicVolumeBinding;
    
    public MapEntity(ISettings settings, PhysicsWorld physicsWorld, string path)
    {
        var fullPath = Path.GetFullPath(path);
        
        // Initialize resources
        source = new AudioSource();
        contentLoader = new ContentLoader(fullPath);
        propContentLoader = new ContentLoader(Path.GetFullPath("Assets/Props"));
        
        // Read the map file
        string json = File.ReadAllText(Path.Combine(fullPath, "map.json"));
        Meta = MapMeta.FromJson(JObject.Parse(json));
        
        // Load the music
        music = contentLoader.Load<AudioStream>(Meta.Music);
        source.AudioStream = music;
        source.Play();
        
        // Bind the music volume to the settings to automatically update volume when changed
        musicVolumeBinding = new Binding<float>(settings, nameof(settings.MusicVolume), source, nameof(AudioSource.Gain));
        
        // Load the props
        var propContent = new PropContent();
        foreach (var propInfo in Meta.Props)
        {
            var location = propContent.Registry[propInfo.Identifier];
            var prop = propContent.Registry[location](propContentLoader, physicsWorld, propInfo.Position, propInfo.Scale, propInfo.Rotation);
            props.Add(prop);
        }
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        foreach (var updateable in props.OfType<IUpdateable>())
        {
            updateable.Update(args);
        }
    }

    public void Render(RenderArgs args)
    {
        foreach (var renderable in props.OfType<IRenderable>())
        {
            renderable.Render(args);
        }
    }

    public void Dispose()
    {
        contentLoader.Dispose();
        propContentLoader.Dispose();
        source.Dispose();

        foreach (var disposable in props.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
        
        musicVolumeBinding.Dispose();
    }
}