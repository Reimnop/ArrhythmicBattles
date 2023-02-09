namespace ArrhythmicBattles.Common.Async;

public delegate Task AsyncCallback(object sender);
public delegate Task AsyncCallback<in T>(object sender, T args);

public class AsyncEvent<T>
{
    private readonly Dictionary<int, AsyncCallback<T>> callbacks = new Dictionary<int, AsyncCallback<T>>();
    private int currentCallbackIndex = 0;

    public AsyncCallbackHandle AddCallback(AsyncCallback<T> callback)
    {
        callbacks.Add(currentCallbackIndex, callback);
        return new AsyncCallbackHandle(currentCallbackIndex++);
    }
    
    public void RemoveCallback(AsyncCallbackHandle callbackHandle)
    {
        callbacks.Remove(callbackHandle.Id);
    }
    
    public async Task InvokeAsync(object sender, T args)
    {
        Task[] tasks = new Task[callbacks.Count];
        int i = 0;
        foreach (AsyncCallback<T> callback in callbacks.Values)
        {
            tasks[i++] = callback(sender, args);
        }
        await Task.WhenAll(tasks);
    }
}

public class AsyncEvent
{
    private readonly Dictionary<int, AsyncCallback> callbacks = new Dictionary<int, AsyncCallback>();
    private int currentCallbackIndex = 0;

    public AsyncCallbackHandle AddCallback(AsyncCallback callback)
    {
        callbacks.Add(currentCallbackIndex, callback);
        return new AsyncCallbackHandle(currentCallbackIndex++);
    }
    
    public void RemoveCallback(AsyncCallbackHandle callbackHandle)
    {
        callbacks.Remove(callbackHandle.Id);
    }
    
    public async Task InvokeAsync(object sender)
    {
        Task[] tasks = new Task[callbacks.Count];
        int i = 0;
        foreach (AsyncCallback callback in callbacks.Values)
        {
            tasks[i++] = callback(sender);
        }
        await Task.WhenAll(tasks);
    }
}