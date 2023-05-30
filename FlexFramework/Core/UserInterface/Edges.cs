namespace FlexFramework.Core.UserInterface;

/// <summary>
/// Represents the edge dimensions.
/// </summary>
public struct Edges
{
    public float Top { get; set; }
    public float Bottom { get; set; }
    public float Left { get; set; }
    public float Right { get; set; }
    
    public Edges(float top, float bottom, float left, float right)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
    }

    public Edges(float value) : this(value, value, value, value)
    {
    }
}