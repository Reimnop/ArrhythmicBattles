using FlexFramework.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Util;

public class InputCapture : IDisposable
{
    private readonly InputSystem system;
    
    public InputCapture(InputSystem system)
    {
        this.system = system;
    }

    public void Dispose()
    {
        system.ReleaseCapture(this);
    }
}

public class InputSystem
{
    public Input Input { get; }
    
    private readonly List<InputCapture> captures = new List<InputCapture>();
    private InputCapture? currentCapture;

    public InputSystem(Input input)
    {
        Input = input;
    }

    public void Update()
    {
        currentCapture = captures.Count > 0 ? captures.Last() : null;
    }

    public InputInfo GetInputInfo()
    {
        return new InputInfo(this, AcquireCapture());
    }

    public InputCapture AcquireCapture()
    {
        InputCapture capture = new InputCapture(this);
        captures.Add(capture);
        return capture;
    }

    public bool IsCurrentCapture(InputCapture capture)
    {
        if (captures.Count == 0)
        {
            return false;
        }
        return currentCapture == capture;
    }

    public void ReleaseCapture(InputCapture capture)
    {
        captures.Remove(capture);
    }
    
    public Vector2 GetMouseDelta(InputCapture capture)
    {
        if (!IsCurrentCapture(capture))
        {
            return Vector2.Zero;
        }
        
        return Input.MouseDelta;
    }
    
    public bool GetMouseDown(InputCapture capture, MouseButton button)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetMouseDown(button);
    }

    public bool GetMouseUp(InputCapture capture, MouseButton button)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetMouseUp(button);
    }
    
    public bool GetMouse(InputCapture capture, MouseButton button)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetMouse(button);
    }

    public bool GetKeyDown(InputCapture capture, Keys key)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetKeyDown(key);
    }
    
    public bool GetKeyUp(InputCapture capture, Keys key)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetKeyUp(key);
    }

    public bool GetKey(InputCapture capture, Keys key)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetKey(key);
    }

    public Vector2 GetMovement(InputCapture capture)
    {
        if (!IsCurrentCapture(capture))
        {
            return Vector2.Zero;
        }

        return Input.GetMovement();
    }

    public bool GetKeyCombo(InputCapture capture, params Keys[] keys)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return Input.GetKeyCombo(keys);
    }
}