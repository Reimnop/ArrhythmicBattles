using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ArrhythmicBattles.Core;

public class GifEntity : ImageEntity, IDisposable // Because who doesn't want to have in-game memes?
{
    private struct Frame
    {
        public Texture2D Texture { get; set; }
        public float Delay { get; set; }
    }
    
    private readonly List<Frame> frames = new List<Frame>();
    private float time = 0.0f;
    private int currentFrame = 0;

    public GifEntity(FlexFrameworkMain engine, string path) : base(engine)
    {
        // Use ImageSharp to load the gif
        using var image = Image.Load<Rgba32>(path);
        
        Rgba32[] pixels = new Rgba32[image.Width * image.Height];

        foreach (var frame in image.Frames)
        {
            frame.CopyPixelDataTo(pixels);

            Texture2D texture = new Texture2D("gif-frame", image.Width, image.Height, SizedInternalFormat.Rgba8);
            texture.LoadData<Rgba32>(pixels, PixelFormat.Rgba, PixelType.UnsignedByte);
            
            int delay = frame.Metadata.GetGifMetadata().FrameDelay;
            
            // Add the frame to the list
            frames.Add(new Frame
            {
                Texture = texture,
                Delay = delay / 100.0f
            });
        }
    }
    
    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        time += args.DeltaTime;
        
        // If the time is greater than the delay of the current frame, move to the next frame
        while (time >= frames[currentFrame].Delay)
        {
            time -= frames[currentFrame].Delay;
            currentFrame = ++currentFrame % frames.Count;
        }
        
        // Set the texture to the current frame
        Texture = frames[currentFrame].Texture;
    }

    public void Dispose()
    {
        foreach (var frame in frames)
        {
            frame.Texture.Dispose();
        }
    }
}