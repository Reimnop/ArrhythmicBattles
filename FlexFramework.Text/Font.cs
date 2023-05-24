namespace FlexFramework.Text;

/// <summary>
/// Represents a font that can be used to render text.
/// </summary>
public class Font
{
    public int GlyphCount => glyphs.Count;
    public int KerningCount => kernings.Count;
    
    public string Name { get; }
    public FontMetrics Metrics { get; }
    public Texture<Rgba8> Texture { get; }
    public GlyphInfo TofuGlyph { get; }
    
    private readonly Dictionary<char, GlyphInfo> glyphs = new();
    private readonly Dictionary<(char, char), int> kernings = new();

    public Font(
        string name, 
        FontMetrics metrics, 
        Texture<Rgba8> texture,
        GlyphInfo tofuGlyph, 
        IDictionary<char, GlyphInfo> glyphs,
        IDictionary<(char, char), int> kernings)
    {
        Name = name;
        Metrics = metrics;
        Texture = texture.Clone();
        this.TofuGlyph = tofuGlyph;
        this.glyphs = glyphs.ToDictionary(x => x.Key, x => x.Value);
        this.kernings = kernings.ToDictionary(x => x.Key, x => x.Value);
    }
    
    public GlyphInfo GetGlyph(char c)
    {
        return glyphs.TryGetValue(c, out GlyphInfo glyph) ? glyph : TofuGlyph;
    }
    
    public int GetKerning(char left, char right)
    {
        return kernings.TryGetValue((left, right), out int kerning) ? kerning : 0;
    }
    
    public IEnumerable<(char, char, int)> GetKernings()
    {
        return kernings.Select(x => (x.Key.Item1, x.Key.Item2, x.Value));
    }
    
    public IEnumerable<(char, GlyphInfo)> GetGlyphs()
    {
        return glyphs.Select(x => (x.Key, x.Value));
    }
}