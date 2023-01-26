namespace ArrhythmicBattles.Common;

public static class TaskHelper
{
    public static async Task WaitUntil(Func<bool> condition, int delay = 5, CancellationToken cancellationToken = default)
    {
        while (!condition() && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(delay, cancellationToken);
        }
    }
}