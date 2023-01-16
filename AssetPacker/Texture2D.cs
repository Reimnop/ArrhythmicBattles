using StbImageSharp;

namespace AssetPacker;

public class Texture2D : IBinarySerializable
{
    public byte[] Data { get; }
    public int Width { get; }
    public int Height { get; }

    public Texture2D(string path)
    {
        using FileStream stream = File.OpenRead(path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        Data = result.Data;
        Width = result.Width;
        Height = result.Height;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(Data);
    }
}