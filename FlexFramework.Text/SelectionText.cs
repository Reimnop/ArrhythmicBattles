namespace FlexFramework.Text;

/// <summary>
/// Represents data used for text selection.
/// </summary>
public class SelectionText
{
    public IReadOnlyList<SelectionLine> Lines { get; }
    
    public SelectionText(IEnumerable<SelectionLine> lines)
    {
        Lines = lines.ToList();
    }
}