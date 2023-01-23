using System.Collections;
using FlexFramework.Core;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Util;

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
    private T targetValue;

    public SimpleAnimator(LerpFunc<T> lerpFunc, ValueConsumer<T>? valueConsumer, T initialValue, float speed)
    {
        this.lerpFunc = lerpFunc;
        this.valueConsumer = valueConsumer;
        this.speed = speed;
        
        currentValue = initialValue;
        targetValue = initialValue;
        valueConsumer?.Invoke(initialValue);
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        t += args.DeltaTime * speed;
        
        if (t < 1.0f)
        {
            currentValue = lerpFunc(currentValue, targetValue, t);
            valueConsumer?.Invoke(currentValue);
        }
        
        if (t >= 1.0f && !Equals(currentValue, targetValue))
        {
            currentValue = targetValue;
            valueConsumer?.Invoke(currentValue);
        }
    }

    public void LerpTo(T targetValue)
    {
        if (Equals(this.targetValue, targetValue))
        {
            return;
        }

        this.targetValue = targetValue;
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