namespace FlexFramework.Text;

/// <summary>
/// Represents data used for text selection.
/// </summary>
public class SelectionText
{
    public IReadOnlyList<SelectionLine> Lines { get; }
    
    // Bounding box
    public int MinX { get; }
    public int MinY { get; }
    public int MaxX { get; }
    public int MaxY { get; }
    
    public SelectionText(IEnumerable<SelectionLine> lines)
    {
        Lines = lines.ToList();

        MinX = Lines.Select(line => line.SelectablePositions).Min(x => x.Min());
        MinY = Lines.Select(line => line.Top).Min();
        MaxX = Lines.Select(line => line.SelectablePositions).Max(x => x.Max());
        MaxY = Lines.Select(line => line.Bottom).Max();

        if (MinY > MaxY)
        {
            
        }
    }
}