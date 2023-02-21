﻿using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core.UserInterface;

public interface IInputProvider
{
    Vector2 MousePosition { get; }
    Vector2 MouseDelta { get; }
    Vector2 MouseScroll { get; }
    Vector2 MouseScrollDelta { get; }
    bool GetMouseDown(MouseButton button);
    bool GetMouseUp(MouseButton button);
    bool GetMouse(MouseButton button);
    bool GetKeyDown(Keys key);
    bool GetKeyUp(Keys key);
    bool GetKey(Keys key);
}