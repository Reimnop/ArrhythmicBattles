using System.Collections;
using FlexFramework.Core;
using FlexFramework.Core.Entities;

namespace ArrhythmicBattles.Util;

public delegate T LerpFunc<T>(T left, T right, float factor);
public delegate void ValueConsumer<T>(T value);

public class SimpleAnimator<T> : Entity where T : struct
{
    public T CurrentValue => currentValue;

    private readonly LerpFunc<T> lerpFunc;
    private readonly ValueConsumer<T>? valueConsumer;

    private readonly float speed;

    private float t = 1.0f;
    private T currentValue;
    private T oldValue;
    private T targetValue;

    public SimpleAnimator(LerpFunc<T> lerpFunc, ValueConsumer<T>? valueConsumer, T initialValue, float speed)
    {
        this.lerpFunc = lerpFunc;
        this.valueConsumer = valueConsumer;
        this.speed = speed;

        currentValue = initialValue;
        oldValue = initialValue;
        targetValue = initialValue;
        valueConsumer?.Invoke(initialValue);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        t += args.DeltaTime * speed;

        if (t < 1.0f)
        {
            currentValue = lerpFunc(oldValue, targetValue, EaseInOut(t));
            valueConsumer?.Invoke(currentValue);
        }
        
        if (t >= 1.0f && !Equals(currentValue, targetValue))
        {
            currentValue = targetValue;
            valueConsumer?.Invoke(currentValue);
        }
    }
    
    // cubic-bezier(0.25, 0.1, 0.25, 1)
    private float EaseInOut(float t)
    {
        return t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
    }

    public void LerpTo(T value)
    {
        if (Equals(targetValue, value))
        {
            return;
        }

        oldValue = currentValue;
        targetValue = value;
        t = 0.0f;
    }

    public void LerpFromTo(T from, T to)
    {
        oldValue = from;
        targetValue = to;
        t = 0.0f;
    }

    public IEnumerator WaitUntilFinish()
    {
        while (t < 1.0)
        {
            yield return null;
        }
    }
}