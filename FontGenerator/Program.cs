using Typography.OpenFont;

// Read OTF file 
using Stream stream = File.OpenRead("C:\\Windows\\Fonts\\arial.ttf");

OpenFontReader reader = new OpenFontReader();
var font = reader.Read(stream);

if (font != null)
{
    // Loop through all glyphs in the font
    for (ushort i = 0; i < font.GlyphCount; i++)
    {
        Glyph glyph = font.GetGlyph(i);

        if (glyph.GlyphClass == GlyphClassKind.Ligature)
        {
            // Get the ligature components
            font.GSUBTable.LookupList
        }
    }
}
