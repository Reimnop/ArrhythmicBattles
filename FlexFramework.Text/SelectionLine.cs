namespace FlexFramework.Text;

/// <summary>
/// Represents a line used for text selection.
/// </summary>
public class SelectionLine
{
    public int Bottom { get; }
    public int Top { get; }
    public IReadOnlyList<int> SelectablePositions => selectablePositions;
    public IReadOnlyList<int> SelectableIndices => selectableIndices;

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