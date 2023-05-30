using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class Element
{
    public abstract Vector2 Size { get; }
    
    protected ElementBox Box { get; private set; }
    
    public virtual void SetBox(ElementBox box)                                                                     
    {
        Box = box;
    }
}