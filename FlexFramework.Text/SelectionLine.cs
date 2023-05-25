namespace FlexFramework.Text;

/// <summary>
/// Represents a line used for text selection.
/// </summary>
public class SelectionLine
{
    public float Bottom { get; }
    public float Top { get; }
    public ReadOnlySpan<float> SelectablePositions => selectablePositions;
    
    private readonly float[] selectablePositions;
    
    public SelectionLine(float top, float bottom, IEnumerable<float> selectablePositions)
    {
        Top = top;
        Bottom = bottom;
        this.selectablePositions = selectablePositions.ToArray();
    }
}