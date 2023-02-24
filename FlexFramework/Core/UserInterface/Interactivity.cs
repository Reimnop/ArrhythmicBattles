using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core.UserInterface;

public delegate void MouseEventHandler(MouseButton button);

public class Interactivity
{
    public event Action? MouseEnter;
    public event Action? MouseLeave;
    public event MouseEventHandler? MouseButtonDown;
    public event MouseEventHandler? MouseButtonUp;
    
    public Bounds Bounds { get; set; }
    
    private readonly IInputProvider inputProvider;

    private bool lastMouseOver;
    private readonly bool[] lastMouseButtons = new bool[(int) MouseButton.Last + 1];
    
    public Interactivity(IInputProvider inputProvider)
    {
        this.inputProvider = inputProvider;
    }

    public void Update()
    {
        bool mouseOver = IsMouseOver();
        
        if (mouseOver && !lastMouseOver)
        {
            MouseEnter?.Invoke();
        }
        else if (!mouseOver && lastMouseOver)
        {
            MouseLeave?.Invoke();
        }
        
        lastMouseOver = mouseOver;
        
        for (int i = 0; i < lastMouseButtons.Length; i++)
        {
            bool mouseButton = IsMouseButton((MouseButton) i);
            
            if (mouseButton && !lastMouseButtons[i])
            {
                MouseButtonDown?.Invoke((MouseButton) i);
            }
            else if (!mouseButton && lastMouseButtons[i])
            {
                MouseButtonUp?.Invoke((MouseButton) i);
            }
            
            lastMouseButtons[i] = mouseButton;
        }
    }
    
    private bool IsMouseOver()
    {
        return inputProvider.InputAvailable && Bounds.Contains(inputProvider.MousePosition);
    }
    
    private bool IsMouseButton(MouseButton button)
    {
        return IsMouseOver() && inputProvider.GetMouse(button);
    }
}