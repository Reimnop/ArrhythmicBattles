using System.Collections;
using FlexFramework.Core.Util;

namespace FlexFramework.Core.EntitySystem;

public abstract class Entity : IDisposable
{
    private HashSet<Coroutine> coroutines = new HashSet<Coroutine>();
    private List<Coroutine> finishedCoroutines = new List<Coroutine>();

    private float deltaTime;

    #region Coroutine Stuff
    
    private bool MoveNext(IEnumerator coroutine)
    {
        if (coroutine.Current is IEnumerator subroutine)
        {
            if (MoveNext(subroutine))
            {
                return true;
            }
        }

        return coroutine.MoveNext();
    }

    protected Coroutine StartCoroutine(IEnumerator enumerator)
    {
        Coroutine coroutine = new Coroutine(enumerator);
        coroutines.Add(coroutine);
        return coroutine;
    }

    protected void StopCoroutine(Coroutine coroutine)
    {
        coroutines.Remove(coroutine);
    }
    
    protected IEnumerator WaitForEndOfFrame()
    {
        yield return null;
    }

    protected IEnumerator WaitForFrames(int frames)
    {
        int i = 0;
        while (i < frames)
        {
            i++;
            yield return null;
        }
    }

    protected IEnumerator WaitForSeconds(float time)
    {
        float t = 0.0f;
        while (t < time)
        {
            t += deltaTime;
            yield return null;
        }
    }

    protected IEnumerator WaitUntil(Func<bool> predicate)
    {
        while (!predicate())
        {
            yield return null;
        }
    }
    
    #endregion

    public virtual void Update(UpdateArgs args)
    {
        deltaTime = args.DeltaTime;

        foreach (Coroutine coroutine in coroutines)
        {
            if (!MoveNext(coroutine.InternalRoutine))
            {
                finishedCoroutines.Add(coroutine);
            }
        }

        foreach (Coroutine coroutine in finishedCoroutines)
        {
            coroutines.Remove(coroutine);
        }
        
        finishedCoroutines.Clear();
    }
    
    public abstract void Dispose();
}