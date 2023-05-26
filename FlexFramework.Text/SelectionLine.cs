namespace FlexFramework.Text;

/// <summary>
/// Represents a line used for text selection.
/// </summary>
public class SelectionLine
{
    public float Bottom { get; }
    public float Top { get; }
    public ReadOnlySpan<float> SelectablePositions => selectablePositions;
    public ReadOnlySpan<int> SelectableIndices => selectableIndices;

    private readonly float[] selectablePositions;
    private readonly int[] selectableIndices;
    
    public SelectionLine(float top, float bottom, IEnumerable<float> selectablePositions, IEnumerable<int> selectableIndices)
    {
        Top = top;
        Bottom = bottom;
        this.selectablePositions = selectablePositions.ToArray();
        this.selectableIndices = selectableIndices.ToArray();
    }
}