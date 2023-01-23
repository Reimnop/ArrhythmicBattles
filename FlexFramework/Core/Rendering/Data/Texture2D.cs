using OpenTK.Graphics.OpenGL4;
using SharpEXR;
using StbImageSharp;
using PixelType = OpenTK.Graphics.OpenGL4.PixelType;

namespace FlexFramework.Core.Rendering.Data;

public class Texture2D : GpuObject, IDisposable
{
    public int Handle { get; }
    public string Name { get; }

    public int Width { get; }
    public int Height { get; }

    public Texture2D(string name, int width, int height, SizedInternalFormat internalFormat)
    {
        Name = name;
        Width = width;
        Height = height;
        
        GL.CreateTextures(TextureTarget.Texture2D, 1, out int handle);
        GL.TextureStorage2D(handle, 1, internalFormat, width, height);
        
        GL.ObjectLabel(ObjectLabelIdentifier.Texture, handle, name.Length, name);
        Handle = handle;
    }

    public static Texture2D FromFile(string name, string path)
    {
        using FileStream stream = File.OpenRead(path);
        return FromStream(name, stream);
    }
    
    public static Texture2D FromStream(string name, Stream stream)
    {
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        Texture2D texture2D = new Texture2D(name, result.Width, result.Height, SizedInternalFormat.Rgba8);
        texture2D.LoadData<byte>(result.Data, PixelFormat.Rgba, PixelType.UnsignedByte);
        texture2D.SetMinFilter(TextureMinFilter.Linear);
        texture2D.SetMagFilter(TextureMagFilter.Linear);
        return texture2D;
    }

    public static Texture2D FromExr(string name, string path)
    {
        EXRFile exrFile = EXRFile.FromFile(path);
        EXRPart part = exrFile.Parts[0];
        part.OpenParallel(path);

        Texture2D texture2D = new Texture2D(name, part.DataWindow.Width, part.DataWindow.Height, SizedInternalFormat.Rgb16f);
        texture2D.LoadData<byte>(part.GetBytes(ImageDestFormat.RGB16, GammaEncoding.Linear), PixelFormat.Rgb, PixelType.HalfFloat);
        texture2D.SetMinFilter(TextureMinFilter.Linear);
        texture2D.SetMagFilter(TextureMagFilter.Linear);
        
        part.Close();

        return texture2D;
    }

    public unsafe void LoadData<T>(ReadOnlySpan<T> data, PixelFormat pixelFormat, PixelType pixelType) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            GL.TextureSubImage2D(Handle, 0, 0, 0, Width, Height, pixelFormat, pixelType, (IntPtr) ptr);
        }
    }
    
    public unsafe void LoadDataPartial<T>(ReadOnlySpan<T> data, int x, int y, int width, int height, PixelFormat pixelFormat, PixelType pixelType) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            GL.TextureSubImage2D(Handle, 0, x, y, width, height, pixelFormat, pixelType, (IntPtr) ptr);
        }
    }

    public void SetMinFilter(TextureMinFilter filter)
    {
        GL.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int) filter);
    }
    
    public void SetMagFilter(TextureMagFilter filter)
    {
        GL.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int) filter);
    }

    public void AnisotropicFiltering(float value)
    {
        GL.TextureParameter(Handle, TextureParameterName.TextureMaxAnisotropy, value);
    }

    public void SetWrapS(TextureWrapMode wrapMode)
    {
        GL.TextureParameter(Handle, TextureParameterName.TextureWrapS, (int) wrapMode);
    }
    
    public void SetWrapT(TextureWrapMode wrapMode)
    {
        GL.TextureParameter(Handle, TextureParameterName.TextureWrapT, (int) wrapMode);
    }
    
    public void GenerateMipmap()
    {
        GL.GenerateTextureMipmap(Handle);
    }

    public void Dispose()
    {
        GL.DeleteTexture(Handle);
    }
}