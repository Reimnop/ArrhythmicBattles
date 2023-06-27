using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Core.Input;

public class KeyboardInputMethod : IInputMethod
{
    private readonly KeyboardState keyboardState;

    public KeyboardInputMethod(KeyboardState keyboardState)
    {
        this.keyboardState = keyboardState;
    }

    public bool GetAttack() => keyboardState.IsKeyDown(Keys.E) && !keyboardState.WasKeyDown(Keys.E);

    public bool GetJump() => keyboardState.IsKeyDown(Keys.Space) && !keyboardState.WasKeyDown(Keys.Space);

    public bool GetPrimarySpecial() => keyboardState.IsKeyDown(Keys.F) && !keyboardState.WasKeyDown(Keys.F);

    public bool GetSecondarySpecial() => keyboardState.IsKeyDown(Keys.V) && !keyboardState.WasKeyDown(Keys.V);

    public Vector2 GetMovement()
    {
        var x = (keyboardState.IsKeyDown(Keys.A) ? -1.0f : 0.0f) + (keyboardState.IsKeyDown(Keys.D) ? 1.0f : 0.0f);
        var y = (keyboardState.IsKeyDown(Keys.S) ? -1.0f : 0.0f) + (keyboardState.IsKeyDown(Keys.W) ? 1.0f : 0.0f);
        return Vector2.Normalize(new Vector2(x, y));
    }
}
