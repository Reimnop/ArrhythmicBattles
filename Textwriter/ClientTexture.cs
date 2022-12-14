using System.Runtime.InteropServices;
using SharpFont;

namespace Textwriter;

public class ClientTexture
{
    public int Width { get; }
    public int Height { get; }
    public byte[] Pixels { get; }
    public int PixelSize { get; }

    public ClientTexture(int width, int height, int pixelSize)
    {
        Pixels = new byte[width * height * pixelSize];
        Width = width;
        Height = height;
        PixelSize = pixelSize;
    }

    public ReadOnlySpan<byte> this[int x, int y]
    {
        get => GetPixel(x, y);
        set => SetPixel(x, y, value);
    }

    private ReadOnlySpan<byte> GetPixel(int x, int y)
    {
        return new ReadOnlySpan<byte>(Pixels, (y * Width + x) * PixelSize, PixelSize);
    }

    private void SetPixel(int x, int y, ReadOnlySpan<byte> value)
    {
        for (int i = 0; i < PixelSize; i++)
        {
            Pixels[(y * Width + x) * PixelSize + i] = value[i];
        }
        
    }

    public void WritePartial(ClientTexture texture, int offsetX, int offsetY)
    {
        WritePartial(texture.Pixels, texture.PixelSize, texture.Width, texture.Height, offsetX, offsetY);
    }

    public void WritePartial(byte[] pixels, int pixelSize, int width, int height, int offsetX, int offsetY)
    {
        if (offsetX + width > Width || offsetY + height > Height ||
            offsetX < 0 || offsetY < 0)
        {
            throw new IndexOutOfRangeException();
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Span<byte> data = stackalloc byte[PixelSize];
                for (int i = 0; i < pixelSize; i++)
                {
                    data[i] = pixels[(y * width + x) * pixelSize + i];
                }
                // Fill the rest of the pixel with 0
                for (int i = pixelSize; i < PixelSize; i++)
                {
                    data[i] = 0;
                }
                SetPixel(x + offsetX, y + offsetY, data);
            }
        }
    }

    public void WritePartial(IntPtr ptr, int width, int height, int offsetX, int offsetY)
    {
        int offsetIndex = (offsetY * Width + offsetX) * PixelSize;
        Marshal.Copy(ptr, Pixels, offsetIndex, width * height * PixelSize);
    }
}