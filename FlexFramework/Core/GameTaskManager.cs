using HalfMaid.Async;

namespace FlexFramework.Core;

public class GameTaskManager : IUpdateable
{
    private GameTaskRunner runner = new();
    private float deltaTime;
    
    public void Update(UpdateArgs args)
    {
        deltaTime = args.DeltaTime;
        runner.RunNextFrame();
    }
    
    public void StartTaskImmediately(Func<GameTask> task)
    {
        runner.StartImmediately(task);
    }

    public GameTaskYieldAwaitable WaitUntilNextFrame() => runner.Next();
    public GameTaskYieldAwaitable DelayFrames(int frames) => runner.Delay(frames);
    public ExternalTaskAwaitable RunTask(Func<Task> task) => runner.RunTask(task);

    public async GameTask WaitSeconds(float seconds)
    {
        float t = 0;
        while (t < seconds)
        {
            t += deltaTime;
            await WaitUntilNextFrame();
        }
    }
    
    public async GameTask RunForSeconds(float seconds, Action<float> task)
    {
        float t = 0;
        while (t < seconds)
        {
            t += deltaTime;
            task(t);
            await WaitUntilNextFrame(); // Prevents infinite loop
        }
    }
    
    public async GameTask RunForSecondsNormalized(float seconds, Action<float> task)
    {
        float t = 0;
        while (t < seconds)
        {
            t += deltaTime;
            task(t / seconds);
            await WaitUntilNextFrame(); // Prevents infinite loop
        }
    }
}