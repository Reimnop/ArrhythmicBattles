using ArrhythmicBattles.Core.IO;
using OpenTK.Windowing.Common.Input;
using SixLabors.ImageSharp.PixelFormats;
using ImageSharpImage = SixLabors.ImageSharp.Image;

namespace ArrhythmicBattles.Core.Resource.Loaders;

public class WindowIconLoader : IResourceLoader
{
    public bool CanLoad(Type type, IFileSystem fileSystem, string path)
    {
        return type == typeof(Image);
    }

    public object Load(Type type, IFileSystem fileSystem, string path)
    {
        using var stream = fileSystem.Open(path, FileMode.Open);
        var image = ImageSharpImage.Load<Rgba32>(stream);
        var pixels = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(pixels);
        return new Image(image.Width, image.Height, pixels);
    }
}