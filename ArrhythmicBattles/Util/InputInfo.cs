namespace ArrhythmicBattles.Util;

public class InputInfo : IDisposable
{
    public InputSystem InputSystem { get; }
    public InputCapture InputCapture { get; }

    public InputInfo(InputSystem inputSystem, InputCapture inputCapture)
    {
        InputSystem = inputSystem;
        InputCapture = inputCapture;
    }

    public void Dispose()
    {
        InputCapture.Dispose();
    }
}