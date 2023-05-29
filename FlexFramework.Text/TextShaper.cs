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
        var minPosY = -glyph.Metrics.HorizontalBearingY;
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

    public static ShapedText ShapeText(
        Font font, 
        string text, 
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, 
        VerticalAlignment verticalAlignment = VerticalAlignment.Top)
    {
        var offsetY = GetTextOffsetY(font, text, verticalAlignment);
        
        var lines = new List<GlyphLine>();
        foreach (var line in text.Split('\n'))
        {
            var offsetX = GetLineOffsetX(font, line, horizontalAlignment);
            lines.Add(ShapeLine(font, line, offsetX, offsetY));
            offsetY += font.Metrics.Height;
        }
        
        return new ShapedText(font, lines);
    }
    
    public static TextBounds GetTextBounds(
        Font font,
        string text,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment verticalAlignment = VerticalAlignment.Top)
    {
        var offsetY = GetTextOffsetY(font, text, verticalAlignment);
        
        var index = -1;
        var lines = new List<LineBounds>();
        foreach (var line in text.Split('\n'))
        {
            var offsetX = GetLineOffsetX(font, line, horizontalAlignment);
            lines.Add(GetLineBounds(font, line, offsetX, offsetY, ref index));
            offsetY += font.Metrics.Height;
        }
        
        return new TextBounds(lines);
    }

    public static LineBounds GetLineBounds(Font font, string line, int offsetX, int offsetY, ref int index)
    {
        var top = offsetY - font.Metrics.Ascent;
        var bottom = offsetY - font.Metrics.Descent;
        if (line.Length == 0)
            return new LineBounds(
                top, 
                bottom, 
                Enumerable.Empty<int>().Append(offsetX), 
                Enumerable.Empty<int>().Append(index++));

        var selectablePositions = new List<int> {offsetX};
        var selectableIndices = new List<int> {index};
        index++;
        var currentX = offsetX;
        for (int i = 0; i < line.Length - 1; i++)
        {
            var left = line[i];
            var right = line[i + 1];
            var glyph = font.GetGlyph(left);
            currentX += glyph.Metrics.AdvanceX + font.GetKerning(left, right);
            selectablePositions.Add(currentX);
            selectableIndices.Add(index);
            index++;
        }

        selectablePositions.Add(currentX + font.GetGlyph(line[^1]).Metrics.AdvanceX);
        selectableIndices.Add(index);
        index++;
        return new LineBounds(top, bottom, selectablePositions, selectableIndices);
    }

    public static GlyphLine ShapeLine(Font font, string line, int x, int y)
    {
        EnsureNoLineBreaks(line);
        
        if (line.Length == 0)
            return new GlyphLine(Enumerable.Empty<ShapedGlyph>()); // Empty line has no glyphs.

        var glyphs = new List<ShapedGlyph>(line.Length);
        var offsetX = x;
        var offsetY = y;
        for (int i = 0; i < line.Length - 1; i++)
        {
            var left = line[i];
            var right = line[i + 1];
            var glyph = font.GetGlyph(left);

            if (glyph.Metrics.Width * glyph.Metrics.Height != 0) // Skip empty glyphs (e.g. spaces).
                glyphs.Add(GetShapedGlyph(glyph, offsetX, offsetY));

            offsetX += glyph.Metrics.AdvanceX + font.GetKerning(left, right);
        }

        var lastGlyph = font.GetGlyph(line[^1]);
        if (lastGlyph.Metrics.Width * lastGlyph.Metrics.Height != 0) // Skip empty glyphs (e.g. spaces).
            glyphs.Add(GetShapedGlyph(lastGlyph, offsetX, offsetY));

        return new GlyphLine(glyphs);
    }

    public static int GetLineOffsetX(Font font, string line, HorizontalAlignment horizontalAlignment)
    {
        EnsureNoLineBreaks(line);

        return horizontalAlignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Center => -(CalculateLineWidth(font, line) >> 1), // Divide by 2
            HorizontalAlignment.Right => -CalculateLineWidth(font, line),
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null)
        };
    }
    
    public static int GetTextOffsetY(Font font, string line, VerticalAlignment verticalAlignment)
    {
        return verticalAlignment switch
        {
            VerticalAlignment.Bottom => -CalculateTextHeight(font, line),
            VerticalAlignment.Center => -(CalculateTextHeight(font, line) >> 1), // Divide by 2
            VerticalAlignment.Top => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(verticalAlignment), verticalAlignment, null)
        };
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