using HalfMaid.Async;

namespace FlexFramework.Core.Entities;

/// <summary>
/// Extension of Entity that adds task based async code support
/// </summary>
public class InteractiveEntity : Entity
{
    protected GameTaskRunner TaskRunner { get; }
    protected float DeltaTime { get; private set; }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        DeltaTime = args.DeltaTime;

        // Update tasks
        TaskRunner.RunNextFrame();
    }
    
    #region Task shenanigans

    protected void StartTaskImmediately(Func<GameTask> task)
    {
        TaskRunner.StartImmediately(task);
    }
    
    protected GameTaskYieldAwaitable WaitUntilNextFrame() => TaskRunner.Next();
    protected GameTaskYieldAwaitable DelayFrames(int frames) => TaskRunner.Delay(frames);
    protected ExternalTaskAwaitable RunTask(Func<Task> task) => TaskRunner.RunTask(task);

    protected async GameTask WaitSeconds(float seconds)
    {
        float t = 0;
        while (t < seconds)
        {
            t += DeltaTime;
            await WaitUntilNextFrame();
        }
    }

    #endregion
}