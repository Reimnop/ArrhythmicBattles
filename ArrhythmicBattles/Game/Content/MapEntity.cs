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
    private readonly ContentLoader content;
    
    private readonly AudioStream music;
    private readonly List<Prop> props = new();
    
    public MapEntity(string path, PropContent propContent)
    {
        string fullPath = Path.GetFullPath(path);
        
        source = new AudioSource();
        content = new ContentLoader(fullPath);
        
        // Read the map file
        string json = File.ReadAllText(Path.Combine(fullPath, "map.json"));
        Meta = MapMeta.FromJson(JObject.Parse(json));
        
        // Load the music
        music = content.Load<AudioStream>(Meta.Music);
        source.AudioStream = music;
        source.Play();
        
        // Load the props
        foreach (var propInfo in Meta.Props)
        {
            var location = propContent.Registry[propInfo.Identifier];
            var prop = propContent.Registry[location](content, propInfo.Position, propInfo.Scale, propInfo.Rotation);
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
        content.Dispose();
        source.Dispose();
        music.Dispose();
        
        foreach (var disposable in props.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}