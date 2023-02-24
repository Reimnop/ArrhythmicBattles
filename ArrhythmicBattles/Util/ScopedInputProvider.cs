﻿using FlexFramework.Core.UserInterface;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Util;

public class ScopedInputProvider : IInputProvider, IDisposable
{
    public bool InputAvailable => InputSystem.IsCurrentCapture(Capture);
    public Vector2 MousePosition => InputSystem.GetMousePosition(Capture);
    public Vector2 MouseDelta => InputSystem.GetMouseDelta(Capture);
    public Vector2 MouseScroll => InputSystem.GetMouseScroll(Capture);
    public Vector2 MouseScrollDelta => InputSystem.GetMouseScrollDelta(Capture);
    
    public InputCapture Capture { get; }
    public InputSystem InputSystem { get; }
    
    public ScopedInputProvider(InputCapture capture, InputSystem inputSystem)
    {
        Capture = capture;
        InputSystem = inputSystem;
    }
    
    public Vector2 GetMovement()
    {
        return InputSystem.GetMovement(Capture);
    }

    public bool GetMouseDown(MouseButton button)
    {
        return InputSystem.GetMouseDown(Capture, button);
    }

    public bool GetMouseUp(MouseButton button)
    {
        return InputSystem.GetMouseUp(Capture, button);
    }

    public bool GetMouse(MouseButton button)
    {
        return InputSystem.GetMouse(Capture, button);
    }

    public bool GetKeyDown(Keys key)
    {
        return InputSystem.GetKeyDown(Capture, key);
    }

    public bool GetKeyUp(Keys key)
    {
        return InputSystem.GetKeyUp(Capture, key);
    }

    public bool GetKey(Keys key)
    {
        return InputSystem.GetKey(Capture, key);
    }

    public void Dispose()
    {
        InputSystem.ReleaseCapture(Capture);
    }
}