using System.Diagnostics;

namespace FlexFramework.Text;

/// <summary>
/// Utility class for shaping text.
/// </summary>
public static class TextShaper
{
    [Conditional("DEBUG")] // Potentially expensive, so only run in debug builds.
    private static void EnsureNoLineBreaks(string line)
    {
        if (line.Contains('\n'))
        {
            throw new ArgumentException("Line must not contain line breaks.", nameof(line));
        }
    }

    private static ShapedGlyph GetShapedGlyph(GlyphInfo glyph, int x, int y)
    {
        var minPosX = glyph.Metrics.HorizontalBearingX;
        var minPosY = glyph.Metrics.HorizontalBearingY;
        var maxPosX = minPosX + glyph.Metrics.Width;
        var maxPosY = minPosY + glyph.Metrics.Height;
        var minTexX = glyph.TextureCoordinates.MinX;
        var minTexY = glyph.TextureCoordinates.MinY;
        var maxTexX = glyph.TextureCoordinates.MaxX;
        var maxTexY = glyph.TextureCoordinates.MaxY;
        minPosX += x;
        minPosY += y;
        maxPosX += x;
        maxPosY += y;
        
        return new ShapedGlyph(
            minPosX, minPosY, maxPosX, maxPosY,
            minTexX, minTexY, maxTexX, maxTexY);
    }

    public static ShapedText ShapeText(Font font, string text)
    {
        var lines = new List<GlyphLine>();
        var offsetX = 0;
        var offsetY = 0;
        foreach (var line in text.Split('\n'))
        {
            lines.Add(ShapeLine(font, line, offsetX, offsetY));
            offsetY += font.Metrics.Height;
        }
        
        return new ShapedText(font, lines);
    }

    public static GlyphLine ShapeLine(Font font, string line, int x, int y)
    {
        EnsureNoLineBreaks(line);
        
        var glyphs = new List<ShapedGlyph>(line.Length);
        var offsetX = x;
        var offsetY = y;
        for (int i = 0; i < line.Length - 1; i++)
        {
            var left = line[i];
            var right = line[i + 1];
            var glyph = font.GetGlyph(left);
            glyphs.Add(GetShapedGlyph(glyph, offsetX, offsetY));
            offsetX += glyph.Metrics.AdvanceX + font.GetKerning(left, right);
        }
        
        var lastGlyph = font.GetGlyph(line[^1]);
        glyphs.Add(GetShapedGlyph(lastGlyph, offsetX, offsetY));
        
        return new GlyphLine(glyphs);
    }

    public static int CalculateTextHeight(Font font, string text)
    {
        var lineBreaks = text.Count(c => c == '\n');
        return font.Metrics.Height * (lineBreaks + 1);
    }
    
    public static int CalculateLineWidth(Font font, string line)
    {
        EnsureNoLineBreaks(line);

        if (line.Length == 0)
            return 0; // Empty line has no width.
        
        if (line.Length == 1)
            return font.GetGlyph(line[0]).Metrics.AdvanceX; // Single character line has the width of that character.
        
        int width = 0;
        for (int i = 0; i < line.Length - 1; i++)
        {
            char left = line[i];
            char right = line[i + 1];
            width += font.GetGlyph(left).Metrics.AdvanceX + font.GetKerning(left, right);
        }
        
        width += font.GetGlyph(line[^1]).Metrics.AdvanceX; // Add the width of the last character.
        return width;
    }
}