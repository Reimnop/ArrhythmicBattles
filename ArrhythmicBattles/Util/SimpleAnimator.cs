﻿using System.Collections;

namespace ArrhythmicBattles.Util;

public delegate T LerpFunc<T>(T left, T right, double factor);
public delegate void ValueConsumer<T>(T value);
public delegate T ValueProvider<T>();

public class SimpleAnimator<T> where T : struct
{
    public T CurrentValue { get; private set; }

    private readonly LerpFunc<T> lerpFunc;
    private readonly ValueConsumer<T>? valueConsumer;

    private readonly double speed = 0.0;

    private double t = 1.0;
    private ValueProvider<T>? valueProvider;
    private T oldValue;

    public SimpleAnimator(LerpFunc<T> lerpFunc, ValueConsumer<T>? valueConsumer, ValueProvider<T> valueProvider, double speed)
    {
        this.lerpFunc = lerpFunc;
        this.valueConsumer = valueConsumer;
        this.valueProvider = valueProvider;
        this.speed = speed;

        oldValue = valueProvider();
    }

    public IEnumerator WaitUntilFinish()
    {
        while (t < 1.0)
        {
            yield return null;
        }
    }

    public void Update(double deltaTime)
    {
        t += deltaTime * speed;

        CurrentValue = lerpFunc(oldValue, valueProvider(), Math.Min(t, 1.0));
        valueConsumer?.Invoke(CurrentValue);
    }

    public void LerpTo(ValueProvider<T> valueProvider)
    {
        oldValue = CurrentValue;
        this.valueProvider = valueProvider;
        t = 0.0;
    }
}