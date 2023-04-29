namespace FlexFramework.Core.Data;

public enum PixelFormat
{
    Rgba,
    Rgb,
    R
}

public enum PixelType
{
    UnsignedByte,
    UnsignedShort,
    UnsignedInt,
    Half,
    Float
}

public class Texture : DataObject
{
    private struct ReadOnlyTexture : ITextureView
    {
        public int Width => texture.Width;
        public int Height => texture.Height;
        public PixelFormat Format => texture.Format;
        public PixelType Type => texture.Type;
        public IBufferView Data => texture.Data.ReadOnly;

        private readonly Texture texture;

        public ReadOnlyTexture(Texture texture)
        {
            this.texture = texture;
        }
    }
    
    public int Width { get; }
    public int Height { get; }
    public PixelFormat Format { get; }
    public PixelType Type { get; }
    public Buffer Data { get; }
    public ITextureView ReadOnly => new ReadOnlyTexture(this);
    
    public Texture(string name, int width, int height, PixelFormat format, PixelType type) : base(name)
    {
        Width = width;
        Height = height;
        Format = format;
        Type = type;
        Data = new Buffer(width * height * GetPixelSize(format, type));
    }
    
    public Texture(string name, int width, int height, PixelFormat format, PixelType type, ReadOnlySpan<byte> data) : this(name, width, height, format, type)
    {
        SetData(data);
    }
    
    public static Texture FromFile(string name, string path)
    {
        using var image = Image.Load<Rgba32>(path);
        Span<Rgba32> pixels = stackalloc Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixels);
        
        var texture = new Texture(name, image.Width, image.Height, PixelFormat.Rgba, PixelType.UnsignedByte);
        texture.SetData<Rgba32>(pixels);
        return texture;
    }

    public void SetData<T>(ReadOnlySpan<T> data) where T : unmanaged
    {
        if (data.Length != GetPixelSize(Format, Type))
            throw new ArgumentException("Data size does not match texture size");
        Data.SetData(data);
    }

    public static int GetPixelSize(PixelFormat format, PixelType type)
    {
        int componentsCount = GetComponentsCount(format);
        int typeSize = GetTypeSize(type);
        return componentsCount * typeSize;
    }

    public static int GetComponentsCount(PixelFormat format)
    {
        return format switch
        {
            PixelFormat.Rgba => 4,
            PixelFormat.Rgb => 3,
            PixelFormat.R => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
    
    public static int GetTypeSize(PixelType type)
    {
        return type switch
        {
            PixelType.UnsignedByte => 1,
            PixelType.UnsignedShort => 2,
            PixelType.UnsignedInt => 4,
            PixelType.Half => 2,
            PixelType.Float => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}