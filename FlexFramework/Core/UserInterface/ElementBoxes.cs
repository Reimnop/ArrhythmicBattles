using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface;

/// <summary>
/// The boxes that define the layout of an element.
/// </summary>
public struct ElementBox
{
    public Box2 MarginBox { get; } // Outer box
    public Box2 BorderBox { get; } // Middle box
    public Box2 ContentBox { get; } // Inner box
    
    public ElementBox(Box2 marginBox, Box2 borderBox, Box2 contentBox)
    {
        MarginBox = marginBox;
        BorderBox = borderBox;
        ContentBox = contentBox;
    }

    public ElementBox Translate(Vector2 translation)
    {
        return new ElementBox(
            MarginBox.Translated(translation), 
            BorderBox.Translated(translation), 
            ContentBox.Translated(translation));
    }
}