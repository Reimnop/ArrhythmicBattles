namespace FlexFramework.Text;

/// <summary>
/// Generates meshes from text.
/// </summary>
public class MeshGenerator
{
    // We don't want to allocate a new array every time we generate a mesh.
    private TextVertex[] vertices;
    private int count;
    
    public MeshGenerator(int capacity = 1024)
    {
        vertices = new TextVertex[capacity];
    }

    /// <summary>
    /// Generates a mesh from the given text.
    /// Will invalidate mesh from previous calls.
    /// </summary>
    /// <returns>The generated text mesh vertices.</returns>
    public ReadOnlySpan<TextVertex> GenerateMesh(ShapedText shapedText)
    {
        const float scale = 1.0f / 64.0f;
        
        Clear(); // Clear previous mesh.
        
        foreach (var line in shapedText.Lines)
        {
            foreach (var shapedGlyph in line)
            {
                var minPosX = shapedGlyph.MinPositionX * scale;
                var minPosY = shapedGlyph.MinPositionY * scale;
                var maxPosX = shapedGlyph.MaxPositionX * scale;
                var maxPosY = shapedGlyph.MaxPositionY * scale;
                var minTexX = shapedGlyph.MinTextureCoordinateX;
                var minTexY = shapedGlyph.MinTextureCoordinateY;
                var maxTexX = shapedGlyph.MaxTextureCoordinateX;
                var maxTexY = shapedGlyph.MaxTextureCoordinateY;
                
                // Triangle 1
                AddVertex(new TextVertex(minPosX, minPosY, minTexX, minTexY));
                AddVertex(new TextVertex(maxPosX, minPosY, maxTexX, minTexY));
                AddVertex(new TextVertex(minPosX, maxPosY, minTexX, maxTexY));
                
                // Triangle 2
                AddVertex(new TextVertex(maxPosX, minPosY, maxTexX, minTexY));
                AddVertex(new TextVertex(maxPosX, maxPosY, maxTexX, maxTexY));
                AddVertex(new TextVertex(minPosX, maxPosY, minTexX, maxTexY));
            }
        }
        
        return new ReadOnlySpan<TextVertex>(vertices, 0, count);
    }
    
    private void AddVertex(TextVertex vertex)
    {
        if (vertices.Length <= count)
        {
            // Double the capacity.
            Array.Resize(ref vertices, count * 2);
        }
        
        vertices[count++] = vertex;
    }

    private void Clear()
    {
        count = 0;
    }
}