using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class Element
{
    public abstract Vector2 Size { get; }

    public abstract void LayoutCallback(ElementBoxes boxes);
}