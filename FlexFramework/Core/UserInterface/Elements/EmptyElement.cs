using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class EmptyElement : Element
{
    public override Vector2 Size => Vector2.Zero;
    
    public override void LayoutCallback(ElementBoxes boxes)
    {
    }
}