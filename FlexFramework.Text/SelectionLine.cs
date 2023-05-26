namespace FlexFramework.Text;

/// <summary>
/// Represents a line used for text selection.
/// </summary>
public class SelectionLine
{
    public int Bottom { get; }
    public int Top { get; }
    public ReadOnlySpan<int> SelectablePositions => selectablePositions;
    public ReadOnlySpan<int> SelectableIndices => selectableIndices;

    private readonly int[] selectablePositions;
    private readonly int[] selectableIndices;
    
    public SelectionLine(int top, int bottom, IEnumerable<int> selectablePositions, IEnumerable<int> selectableIndices)
    {
        Top = top;
        Bottom = bottom;
        this.selectablePositions = selectablePositions.ToArray();
        this.selectableIndices = selectableIndices.ToArray();
    }
}