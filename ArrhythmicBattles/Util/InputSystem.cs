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
    private readonly Stack<InputCapture> captures = new Stack<InputCapture>();
    private readonly Input input;

    public InputSystem(Input input)
    {
        this.input = input;
    }

    public InputCapture AcquireCapture()
    {
        InputCapture capture = new InputCapture(this);
        captures.Push(capture);
        return capture;
    }

    public bool IsCurrentCapture(InputCapture capture)
    {
        if (captures.Count == 0)
        {
            return false;
        }
        return captures.Peek() == capture;
    }

    public void ReleaseCapture(InputCapture capture)
    {
        if (!IsCurrentCapture(capture))
        {
            throw new Exception("Attempted to release a capture that is not on top");
        }

        captures.Pop();
    }
    
    public bool GetMouseDown(InputCapture capture, MouseButton button)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return input.GetMouseDown(button);
    }

    public bool GetMouseUp(InputCapture capture, MouseButton button)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return input.GetMouseUp(button);
    }
    
    public bool GetMouse(InputCapture capture, MouseButton button)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return input.GetMouse(button);
    }

    public bool GetKeyDown(InputCapture capture, Keys key)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return input.GetKeyDown(key);
    }
    
    public bool GetKeyUp(InputCapture capture, Keys key)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return input.GetKeyUp(key);
    }

    public bool GetKey(InputCapture capture, Keys key)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return input.GetKey(key);
    }

    public Vector2 GetMovement(InputCapture capture)
    {
        if (!IsCurrentCapture(capture))
        {
            return Vector2.Zero;
        }

        return input.GetMovement();
    }

    public bool GetKeyCombo(InputCapture capture, params Keys[] keys)
    {
        if (!IsCurrentCapture(capture))
        {
            return false;
        }

        return input.GetKeyCombo(keys);
    }
}