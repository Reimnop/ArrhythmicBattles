using FlexFramework.Core.UserInterface.Drawables;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core.UserInterface.Animators;

public class InteractiveAnimator
{
    protected IInputProvider InputProvider { get; }
    
    private bool lastLeftMouseState;
    private Vector2 lastMousePosition;
    
    public InteractiveAnimator(IInputProvider inputProvider)
    {
        InputProvider = inputProvider;
    }
    
    public virtual void Update(Drawable drawable, float deltaTime)
    {
        Bounds bounds = drawable.Bounds;
        Vector2 mousePosition = InputProvider.MousePosition;
        
        // TODO: Implement more cases, I was too lazy to do it right now, also it was 1 AM when I wrote this, and I need to sleep.
        
        if (bounds.Contains(mousePosition) && !bounds.Contains(lastMousePosition))
            OnMouseEnter(drawable);
        else if (!bounds.Contains(mousePosition) && bounds.Contains(lastMousePosition))
            OnMouseLeave(drawable);
        
        bool leftMouseState = InputProvider.GetMouse(MouseButton.Left) && bounds.Contains(mousePosition);

        if (leftMouseState && !lastLeftMouseState)
            OnMouseDown(drawable, MouseButton.Left);
        else if (!leftMouseState && lastLeftMouseState)
            OnMouseUp(drawable, MouseButton.Left);

        lastMousePosition = InputProvider.MousePosition;
        lastLeftMouseState = leftMouseState;
    }
    
    public virtual void OnMouseEnter(Drawable drawable)
    {
    }
    
    public virtual void OnMouseLeave(Drawable drawable)
    {
    }
    
    public virtual void OnMouseDown(Drawable drawable, MouseButton button)
    {
    }
    
    public virtual void OnMouseUp(Drawable drawable, MouseButton button)
    {
    }
}