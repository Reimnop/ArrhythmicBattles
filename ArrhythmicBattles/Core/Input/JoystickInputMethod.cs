using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Core.Input;

public class JoystickInputMethod : IInputMethod
{
    private readonly JoystickState joystickState;
    
    public JoystickInputMethod(JoystickState joystickState)
    {
        this.joystickState = joystickState;
    }
    
    public bool GetJump()
    {
        return joystickState.IsButtonDown(0) && !joystickState.WasButtonDown(0); // Button "A"
    }

    public bool GetAttack()
    {
        return false;
    }

    public bool GetPrimarySpecial()
    {
        return false;
    }

    public bool GetSecondarySpecial()
    {
        return false;
    }

    public Vector2 GetMovement()
    {
        const float deadzone = 0.2f;
        
        var x = joystickState.GetAxis(0);
        var y = -joystickState.GetAxis(1);
        var movement = new Vector2(x, y);
        return movement.Length < deadzone ? Vector2.Zero : movement;
    }
}