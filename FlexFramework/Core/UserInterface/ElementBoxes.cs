using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

/// <summary>
/// Represent the boxes that define the layout of an element.
/// </summary>
public struct ElementBoxes
{
    public Box2 MarginBox { get; } // Outer box
    public Box2 BorderBox { get; } // Middle box
    public Box2 ContentBox { get; } // Inner box
    
    public ElementBoxes(Box2 marginBox, Box2 borderBox, Box2 contentBox)
    {
        MarginBox = marginBox;
        BorderBox = borderBox;
        ContentBox = contentBox;
    }
    
    public ElementBoxes(Box2 box) : this(box, box, box)
    {
    }

    public ElementBoxes Translate(Vector2 translation)
    {
        return new ElementBoxes(
            MarginBox.Translated(translation), 
            BorderBox.Translated(translation), 
            ContentBox.Translated(translation));
    }
}