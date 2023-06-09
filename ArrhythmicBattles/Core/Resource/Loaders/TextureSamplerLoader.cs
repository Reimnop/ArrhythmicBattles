using ArrhythmicBattles.Core.IO;
using FlexFramework.Core.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ArrhythmicBattles.Core.Resource.Loaders;

public class TextureSamplerLoader : IResourceLoader
{
    public bool CanLoad(Type type, IFileSystem fileSystem, string path)
    {
        return type == typeof(TextureSampler);
    }

    public object Load(Type type, IFileSystem fileSystem, string path)
    {
        using var stream = fileSystem.Open(path, FileMode.Open);
        using var image = Image.Load<Rgba32>(stream);
        var pixels = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(pixels);
        var name = Path.GetFileNameWithoutExtension(path);
        var texture = new Texture(name, image.Width, image.Height, PixelFormat.Rgba8, pixels);
        var sampler = new Sampler(name);
        return new TextureSampler(texture, sampler);
    }
}