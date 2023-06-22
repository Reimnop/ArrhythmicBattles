using OpenTK.Mathematics;

namespace ArrhythmicBattles.Core.Input;

public interface IInputMethod
{
    bool GetJump();
    bool GetAttack();
    bool GetPrimarySpecial();
    bool GetSecondarySpecial();
    Vector2 GetMovement();
}
