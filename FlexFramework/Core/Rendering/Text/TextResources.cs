using FlexFramework.Core.Rendering.Data;
using Msdfgen;
using OpenTK.Graphics.OpenGL4;
using SharpFont;
using Textwriter;

namespace FlexFramework.Core.Rendering.Text;

public class TextResources : IDisposable
{
    public Font[] Fonts { get; }
    public Texture2D[] FontTextures { get; }

    private readonly Library freetype;
    private readonly Dictionary<string, int> nameToIndex;

    public TextResources(int atlasWidth, params FontFileInfo[] fontFiles)
    {
        freetype = new Library();
        nameToIndex = new Dictionary<string, int>();

        Fonts = new Font[fontFiles.Length];
        for (int i = 0; i < fontFiles.Length; i++)
        {
            Fonts[i] = new Font(freetype, fontFiles[i].Path, fontFiles[i].Size, atlasWidth);
            nameToIndex.Add(fontFiles[i].Name, i);
        }

        List<Texture2D> textures = new List<Texture2D>();
        foreach (Font font in Fonts)
        {
            AtlasTexture<FloatRgb> atlasTexture = font.Atlas;
            Texture2D texture = new Texture2D($"{font.FamilyName}-atlas", atlasTexture.Texture.Width, atlasTexture.Texture.Height, SizedInternalFormat.Rgb32f);
            texture.LoadData<FloatRgb>(atlasTexture.Texture.Pixels, PixelFormat.Rgb, PixelType.Float);
            texture.SetMinFilter(TextureMinFilter.Linear);
            texture.SetMagFilter(TextureMagFilter.Linear);
            textures.Add(texture);
        }

        FontTextures = textures.ToArray();
    }

    public Font GetFont(string name)
    {
        return Fonts[nameToIndex[name]];
    }
    
    public void Dispose()
    {
        foreach (Font font in Fonts)
        {
            font.Dispose();
        }
        
        foreach (Texture2D texture in FontTextures)
        {
            texture.Dispose();
        }
        
        freetype.Dispose();
    }
}
