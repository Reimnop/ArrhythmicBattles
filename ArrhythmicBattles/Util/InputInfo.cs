namespace ArrhythmicBattles.Util;

public struct InputInfo
{
    public InputSystem InputSystem { get; }
    public InputCapture InputCapture { get; }

    public InputInfo(InputSystem inputSystem, InputCapture inputCapture)
    {
        InputSystem = inputSystem;
        InputCapture = inputCapture;
    }
}