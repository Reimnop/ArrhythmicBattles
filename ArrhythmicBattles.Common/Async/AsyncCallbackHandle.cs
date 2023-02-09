namespace ArrhythmicBattles.Common.Async;

public struct AsyncCallbackHandle
{
    public int Id { get; }
    
    public AsyncCallbackHandle(int id)
    {
        Id = id;
    }
}