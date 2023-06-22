using ArrhythmicBattles.Core.Input;
using FlexFramework.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Core.Input;

public class KeyboardInputMethod : IInputMethod
{
    private readonly IInputProvider inputProvider;

    public KeyboardInputMethod(IInputProvider inputProvider)
    {
        this.inputProvider = inputProvider;
    }

    public bool GetAttack() => inputProvider.GetKeyDown(Keys.E);

    public bool GetJump() => inputProvider.GetKeyDown(Keys.Space);

    public bool GetPrimarySpecial() => inputProvider.GetKeyDown(Keys.F);

    public bool GetSecondarySpecial() => inputProvider.GetKeyDown(Keys.V);

    public Vector2 GetMovement() => inputProvider.Movement;
}
