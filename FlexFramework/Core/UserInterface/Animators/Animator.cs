using FlexFramework.Core.UserInterface.Drawables;

namespace FlexFramework.Core.UserInterface.Animators;

public abstract class Animator
{
    public abstract void Update(Drawable drawable, float deltaTime);
}