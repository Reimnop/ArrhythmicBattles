using FlexFramework.Core.UserInterface.Drawables;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core.UserInterface.Animators;

public class InteractiveAnimator
{
    protected IInputProvider InputProvider { get; }
    
    private Vector2 lastMousePosition;
    
    public InteractiveAnimator(IInputProvider inputProvider)
    {
        InputProvider = inputProvider;
    }
    
    public virtual void Update(Drawable drawable, float deltaTime)
    {
        Bounds bounds = drawable.Bounds;
        Vector2 mousePosition = InputProvider.MousePosition;
        
        if (bounds.Contains(mousePosition) && !bounds.Contains(lastMousePosition))
            OnMouseEnter(drawable);
        else if (!bounds.Contains(mousePosition) && bounds.Contains(lastMousePosition))
            OnMouseLeave(drawable);

        if (InputProvider.GetMouseDown(MouseButton.Left) && bounds.Contains(mousePosition))
        {
            OnMouseDown(drawable, MouseButton.Left);
        }
        
        if (InputProvider.GetMouseUp(MouseButton.Left))
        {
            OnMouseUp(drawable, MouseButton.Left);
        }
        
        if (InputProvider.GetMouse(MouseButton.Left) && bounds.Contains(mousePosition))
        {
            OnMouseClick(drawable, MouseButton.Left);
        }

        lastMousePosition = InputProvider.MousePosition;
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
    
    public virtual void OnMouseClick(Drawable drawable, MouseButton button)
    {
    }
}