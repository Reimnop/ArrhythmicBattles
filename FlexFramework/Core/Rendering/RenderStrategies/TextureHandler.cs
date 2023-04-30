using System.Diagnostics;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using Buffer = FlexFramework.Core.Rendering.Data.Buffer;
using PixelFormat = FlexFramework.Core.Data.PixelFormat;
using PixelType = FlexFramework.Core.Data.PixelType;

namespace FlexFramework.Core.Rendering.RenderStrategies;

// TODO: This is dumb. Don't use this in production.
// Someone should probably implement a GC for this. A simple one would be fine.
public class TextureHandler
{
    // This is extremely verbose
    private Dictionary<(PixelFormat, PixelType), SizedInternalFormat> formatTypeMap = new()
    {
        {(PixelFormat.Rgba, PixelType.UnsignedByte), SizedInternalFormat.Rgba8},
        {(PixelFormat.Rgba, PixelType.UnsignedShort), SizedInternalFormat.Rgba16},
        {(PixelFormat.Rgba, PixelType.UnsignedInt), SizedInternalFormat.Rgba32ui},
        {(PixelFormat.Rgba, PixelType.Half), SizedInternalFormat.Rgba16f},
        {(PixelFormat.Rgba, PixelType.Float), SizedInternalFormat.Rgba32f},
        {(PixelFormat.Rgb, PixelType.UnsignedByte), SizedInternalFormat.Rgb8},
        {(PixelFormat.Rgb, PixelType.UnsignedShort), SizedInternalFormat.Rgb16ui},
        {(PixelFormat.Rgb, PixelType.UnsignedInt), SizedInternalFormat.Rgb32ui},
        {(PixelFormat.Rgb, PixelType.Half), SizedInternalFormat.Rgb16f},
        {(PixelFormat.Rgb, PixelType.Float), SizedInternalFormat.Rgb32f},
        {(PixelFormat.R, PixelType.UnsignedByte), SizedInternalFormat.R8},
        {(PixelFormat.R, PixelType.UnsignedShort), SizedInternalFormat.R16},
        {(PixelFormat.R, PixelType.UnsignedInt), SizedInternalFormat.R32ui},
        {(PixelFormat.R, PixelType.Half), SizedInternalFormat.R16f},
        {(PixelFormat.R, PixelType.Float), SizedInternalFormat.R32f}
    };
    
    private Dictionary<PixelFormat, OpenTK.Graphics.OpenGL4.PixelFormat> formatMap = new()
    {
        {PixelFormat.Rgba, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba},
        {PixelFormat.Rgb, OpenTK.Graphics.OpenGL4.PixelFormat.Rgb},
        {PixelFormat.R, OpenTK.Graphics.OpenGL4.PixelFormat.Red}
    };
    
    private Dictionary<PixelType, OpenTK.Graphics.OpenGL4.PixelType> typeMap = new()
    {
        {PixelType.UnsignedByte, OpenTK.Graphics.OpenGL4.PixelType.UnsignedByte},
        {PixelType.UnsignedShort, OpenTK.Graphics.OpenGL4.PixelType.UnsignedShort},
        {PixelType.UnsignedInt, OpenTK.Graphics.OpenGL4.PixelType.UnsignedInt},
        {PixelType.Half, OpenTK.Graphics.OpenGL4.PixelType.HalfFloat},
        {PixelType.Float, OpenTK.Graphics.OpenGL4.PixelType.Float}
    };

    public Texture2D GetTexture(ITextureView texture)
    {
        SizedInternalFormat internalFormat = ConvertToSizedInternalFormat(texture.Format, texture.Type);
        Texture2D tex = new("texture", texture.Width, texture.Height, internalFormat);
        tex.SetData(texture.Data.Data, ConvertToPixelFormat(texture.Format), ConvertToPixelType(texture.Type));
        
        return tex;
    }
    
    private SizedInternalFormat ConvertToSizedInternalFormat(PixelFormat pixelFormat, PixelType pixelType)
    {
        if (formatTypeMap.TryGetValue((pixelFormat, pixelType), out var internalFormat))
        {
            return internalFormat;
        }
    
        throw new ArgumentException("Invalid pixel format or type");
    }
    
    private OpenTK.Graphics.OpenGL4.PixelFormat ConvertToPixelFormat(PixelFormat pixelFormat)
    {
        if (formatMap.TryGetValue(pixelFormat, out var openGLPixelFormat))
        {
            return openGLPixelFormat;
        }
    
        throw new ArgumentException("Invalid pixel format");
    }
    
    private OpenTK.Graphics.OpenGL4.PixelType ConvertToPixelType(PixelType pixelType)
    {
        if (typeMap.TryGetValue(pixelType, out var openGLPixelType))
        {
            return openGLPixelType;
        }
    
        throw new ArgumentException("Invalid pixel type");
    }
}