using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core.UserInterface;

public delegate void MouseEventHandler(MouseButton button);

public class Interactivity : IUpdateable // In case you want to cast it to IUpdateable (don't do that)
{
    public event Action? MouseEnter;
    public event Action? MouseLeave;
    public event MouseEventHandler? MouseButtonDown;
    public event MouseEventHandler? MouseButtonUp;
    
    public Bounds Bounds { get; set; }
    public bool MouseOver { get; private set; }
    public bool[] MouseButtons { get; } = new bool[(int) MouseButton.Last + 1];
    
    private readonly IInputProvider inputProvider;

    private bool lastMouseOver;
    private readonly bool[] lastMouseButtons = new bool[(int) MouseButton.Last + 1];
    
    public Interactivity(IInputProvider inputProvider)
    {
        this.inputProvider = inputProvider;
    }

    public void Update()
    {
        // Get input
        MouseOver = IsMouseOver();
        for (int i = 0; i < MouseButtons.Length; i++)
        {
            MouseButtons[i] = IsMouseButton((MouseButton) i) && MouseOver;
        }
        
        // Check for mouse enter/leave
        if (MouseOver && !lastMouseOver)
        {
            MouseEnter?.Invoke();
        }
        else if (!MouseOver && lastMouseOver)
        {
            MouseLeave?.Invoke();
        }

        for (int i = 0; i < MouseButtons.Length; i++)
        {
            if (inputProvider.GetMouseDown((MouseButton) i) && MouseOver)
            {
                MouseButtonDown?.Invoke((MouseButton) i);
            }
            else if (inputProvider.GetMouseUp((MouseButton) i) && MouseOver)
            {
                MouseButtonUp?.Invoke((MouseButton) i);
            }
        }
        
        // Update last values
        lastMouseOver = MouseOver;
    }
    
    public void Update(UpdateArgs args)
    {
        Update();
    }
    
    private bool IsMouseOver()
    {
        return inputProvider.InputAvailable && Bounds.Contains(inputProvider.MousePosition);
    }
    
    private bool IsMouseButton(MouseButton button)
    {
        return inputProvider.InputAvailable && inputProvider.GetMouse(button);
    }
}