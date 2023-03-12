using System.Runtime.CompilerServices;

namespace ArrhythmicBattles.Core.Animation;

public class NoKeyException : Exception
{
    public NoKeyException() : base("Cannot interpolate in an empty sequence!")
    {
    }
}

public delegate T Interpolator<T>(T first, T second, float factor);

public class Sequence<T> where T : struct
{
    public T CurrentValue { get; private set; }

    private readonly IReadOnlyList<Key<T>> keys;
    private readonly Interpolator<T> interpolator;

    private float lastTime = 0.0f;
    private int index = 0;

    public Sequence(IReadOnlyList<Key<T>> keys, Interpolator<T> interpolator)
    {
        this.interpolator = interpolator;
        this.keys = keys;
    }
    
    public T Interpolate(float time)
    {
        if (keys.Count == 0)
        {
            throw new NoKeyException();
        }

        if (keys.Count == 1)
        {
            return SetCurrentValue(ResultFromSingleKey(keys[0]));
        }

        if (time < keys[0].Time)
        {
            return SetCurrentValue(ResultFromSingleKey(keys[0]));
        }
        
        if (time >= keys[^1].Time)
        {
            return SetCurrentValue(ResultFromSingleKey(keys[^1]));
        }

        int step = time >= lastTime ? 1 : -1;
        lastTime = time;

        while (!(time >= keys[index].Time && time < keys[index + 1].Time))
        {
            index += step;
        }

        Key<T> first = keys[index];
        Key<T> second = keys[index + 1];
        
        float t = InverseLerp(first.Time, second.Time, time);
        return SetCurrentValue(interpolator(first.Value, second.Value, t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T SetCurrentValue(T currentValue)
    {
        CurrentValue = currentValue;
        return currentValue;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T ResultFromSingleKey(Key<T> key)
    {
        return interpolator(key.Value, key.Value, 0.0f);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float InverseLerp(float first, float second, float value)
    {
        return (value - first) / (second - first);
    }
}