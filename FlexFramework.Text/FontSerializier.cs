using System.Text;

namespace FlexFramework.Text;

/// <summary>
/// Used to serialize fonts to a binary file.
/// </summary>
public static class FontSerializier
{
    public static void SerializeFont(Font font, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
        
        writer.Write(font.Name);
        WriteFontMetrics(font.Metrics, writer);
        WriteTexture(font.Texture, writer);

        // Write glyphs
        WriteGlyphInfo(font.TofuGlyph, writer); // Write tofu glyph
        writer.Write(font.GlyphCount);
        foreach (var (c, glyph) in font.GetGlyphs())
        {
            writer.Write(c);
            WriteGlyphInfo(glyph, writer);
        }
        
        // Write kernings
        writer.Write(font.KerningCount);
        foreach (var (left, right, kerning) in font.GetKernings())
        {
            writer.Write(left);
            writer.Write(right);
            writer.Write(kerning);
        }
    }

    private static void WriteFontMetrics(FontMetrics metrics, BinaryWriter writer)
    {
        writer.Write(metrics.Size);
        writer.Write(metrics.Height);
        writer.Write(metrics.Ascent);
        writer.Write(metrics.Descent);
    }
    
    private static unsafe void WriteTexture(Texture<Rgba8> texture, BinaryWriter writer)
    {
        writer.Write(texture.Width);
        writer.Write(texture.Height);
        
        fixed (Rgba8* pixels = texture.Pixels)
        {
            var span = new ReadOnlySpan<byte>(pixels, texture.Pixels.Length * sizeof(Rgba8));
            writer.Write(span);
        }
    }
    
    private static void WriteGlyphInfo(GlyphInfo glyph, BinaryWriter writer)
    {
        WriteGlyphMetrics(glyph.Metrics, writer);
        WriteTextureCoordinates(glyph.TextureCoordinates, writer);
    }
    
    private static void WriteGlyphMetrics(GlyphMetrics metrics, BinaryWriter writer)
    {
        writer.Write(metrics.Width);
        writer.Write(metrics.Height);
        writer.Write(metrics.AdvanceX);
        writer.Write(metrics.AdvanceY);
        writer.Write(metrics.HorizontalBearingX);
        writer.Write(metrics.HorizontalBearingY);
        writer.Write(metrics.VerticalBearingX);
        writer.Write(metrics.VerticalBearingY);
    }
    
    private static void WriteTextureCoordinates(TextureCoordinates coords, BinaryWriter writer)
    {
        writer.Write(coords.MinX);
        writer.Write(coords.MinY);
        writer.Write(coords.MaxX);
        writer.Write(coords.MaxY);
    }
}