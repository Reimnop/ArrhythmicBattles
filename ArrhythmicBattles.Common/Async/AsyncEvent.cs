namespace ArrhythmicBattles.Common.Async;

public delegate Task AsyncCallback(object sender);
public delegate Task AsyncCallback<in T>(object sender, T args);

public class AsyncEvent<T>
{
    private readonly HashSet<AsyncCallback<T>> callbacks = new HashSet<AsyncCallback<T>>();

    public void AddCallback(AsyncCallback<T> callback)
    {
        callbacks.Add(callback);
    }
    
    public void RemoveCallback(AsyncCallback<T> callbackHandle)
    {
        callbacks.Remove(callbackHandle);
    }
    
    public async Task InvokeAsync(object sender, T args)
    {
        Task[] tasks = new Task[callbacks.Count];
        int i = 0;
        foreach (AsyncCallback<T> callback in callbacks)
        {
            tasks[i++] = callback(sender, args);
        }
        await Task.WhenAll(tasks);
    }
}

public class AsyncEvent
{
    private readonly HashSet<AsyncCallback> callbacks = new HashSet<AsyncCallback>();

    public void AddCallback(AsyncCallback callback)
    {
        callbacks.Add(callback);
    }
    
    public void RemoveCallback(AsyncCallback callbackHandle)
    {
        callbacks.Remove(callbackHandle);
    }
    
    public async Task InvokeAsync(object sender)
    {
        Task[] tasks = new Task[callbacks.Count];
        int i = 0;
        foreach (AsyncCallback callback in callbacks)
        {
            tasks[i++] = callback(sender);
        }
        await Task.WhenAll(tasks);
    }
}