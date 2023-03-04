namespace Glide;

/// <summary>
///     Static class with useful easer functions that can be used by Tweens.
/// </summary>
public static class Ease
{
    private const float PI = 3.14159f;
    private const float PI2 = PI / 2;
    private const float B1 = 1 / 2.75f;
    private const float B2 = 2 / 2.75f;
    private const float B3 = 1.5f / 2.75f;
    private const float B4 = 2.5f / 2.75f;
    private const float B5 = 2.25f / 2.75f;
    private const float B6 = 2.625f / 2.75f;

    /// <summary>
    ///     Ease a value to its target and then back. Use this to wrap another easing function.
    /// </summary>
    public static Func<float, float> ToAndFro(Func<float, float> easer)
    {
        return t => ToAndFro(easer(t));
    }

    /// <summary>
    ///     Ease a value to its target and then back.
    /// </summary>
    public static float ToAndFro(float t)
    {
        return t < 0.5f ? t * 2 : 1 + (t - 0.5f) / 0.5f * -1;
    }

    /// <summary>
    ///     Elastic in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float ElasticIn(float t)
    {
        return MathF.Sin(13 * PI2 * t) * MathF.Pow(2, 10 * (t - 1));
    }

    /// <summary>
    ///     Elastic out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float ElasticOut(float t)
    {
        if (t == 1) return 1;
        return MathF.Sin(-13 * PI2 * (t + 1)) * MathF.Pow(2, -10 * t) + 1;
    }

    /// <summary>
    ///     Elastic in and out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float ElasticInOut(float t)
    {
        if (t < 0.5f) return 0.5f * MathF.Sin(13 * PI2 * (2 * t)) * MathF.Pow(2, 10 * (2 * t - 1));

        return 0.5f * (MathF.Sin(-13 * PI2 * (2 * t - 1 + 1)) * MathF.Pow(2, -10 * (2 * t - 1)) + 2);
    }

    /// <summary>
    ///     Quadratic in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float QuadIn(float t)
    {
        return t * t;
    }

    /// <summary>
    ///     Quadratic out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float QuadOut(float t)
    {
        return -t * (t - 2);
    }

    /// <summary>
    ///     Quadratic in and out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float QuadInOut(float t)
    {
        return t <= .5 ? t * t * 2 : 1 - --t * t * 2;
    }

    /// <summary>
    ///     Cubic in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float CubeIn(float t)
    {
        return t * t * t;
    }

    /// <summary>
    ///     Cubic out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float CubeOut(float t)
    {
        return 1 + --t * t * t;
    }

    /// <summary>
    ///     Cubic in and out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float CubeInOut(float t)
    {
        return t <= .5 ? t * t * t * 4 : 1 + --t * t * t * 4;
    }

    /// <summary>
    ///     Quart in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float QuartIn(float t)
    {
        return t * t * t * t;
    }

    /// <summary>
    ///     Quart out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float QuartOut(float t)
    {
        return 1 - (t -= 1) * t * t * t;
    }

    /// <summary>
    ///     Quart in and out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float QuartInOut(float t)
    {
        return t <= .5f ? t * t * t * t * 8 : (1 - (t = t * 2 - 2) * t * t * t) / 2 + .5f;
    }

    /// <summary>
    ///     Quint in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float QuintIn(float t)
    {
        return t * t * t * t * t;
    }

    /// <summary>
    ///     Quint out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float QuintOut(float t)
    {
        return (t = t - 1) * t * t * t * t + 1;
    }

    /// <summary>
    ///     Quint in and out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float QuintInOut(float t)
    {
        return (t *= 2) < 1 ? t * t * t * t * t / 2 : ((t -= 2) * t * t * t * t + 2) / 2;
    }

    /// <summary>
    ///     Sine in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float SineIn(float t)
    {
        if (t == 1) return 1;
        return -MathF.Cos(PI2 * t) + 1;
    }

    /// <summary>
    ///     Sine out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float SineOut(float t)
    {
        return MathF.Sin(PI2 * t);
    }

    /// <summary>
    ///     Sine in and out
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float SineInOut(float t)
    {
        return -MathF.Cos(PI * t) / 2 + .5f;
    }

    /// <summary>
    ///     Bounce in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float BounceIn(float t)
    {
        t = 1 - t;
        if (t < B1) return 1 - 7.5625f * t * t;
        if (t < B2) return 1 - (7.5625f * (t - B3) * (t - B3) + .75f);
        if (t < B4) return 1 - (7.5625f * (t - B5) * (t - B5) + .9375f);
        return 1 - (7.5625f * (t - B6) * (t - B6) + .984375f);
    }

    /// <summary>
    ///     Bounce out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float BounceOut(float t)
    {
        if (t < B1) return 7.5625f * t * t;
        if (t < B2) return 7.5625f * (t - B3) * (t - B3) + .75f;
        if (t < B4) return 7.5625f * (t - B5) * (t - B5) + .9375f;
        return 7.5625f * (t - B6) * (t - B6) + .984375f;
    }

    /// <summary>
    ///     Bounce in and out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float BounceInOut(float t)
    {
        if (t < .5)
        {
            t = 1 - t * 2;
            if (t < B1) return (1 - 7.5625f * t * t) / 2;
            if (t < B2) return (1 - (7.5625f * (t - B3) * (t - B3) + .75f)) / 2;
            if (t < B4) return (1 - (7.5625f * (t - B5) * (t - B5) + .9375f)) / 2;
            return (1 - (7.5625f * (t - B6) * (t - B6) + .984375f)) / 2;
        }

        t = t * 2 - 1;
        if (t < B1) return 7.5625f * t * t / 2 + .5f;
        if (t < B2) return (7.5625f * (t - B3) * (t - B3) + .75f) / 2 + .5f;
        if (t < B4) return (7.5625f * (t - B5) * (t - B5) + .9375f) / 2 + .5f;
        return (7.5625f * (t - B6) * (t - B6) + .984375f) / 2 + .5f;
    }

    /// <summary>
    ///     Circle in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float CircIn(float t)
    {
        return -(MathF.Sqrt(1 - t * t) - 1);
    }

    /// <summary>
    ///     Circle out
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float CircOut(float t)
    {
        return MathF.Sqrt(1 - (t - 1) * (t - 1));
    }

    /// <summary>
    ///     Circle in and out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float CircInOut(float t)
    {
        return t <= .5f
            ? (MathF.Sqrt(1 - t * t * 4) - 1) / -2
            : (MathF.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) / 2;
    }

    /// <summary>
    ///     Exponential in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float ExpoIn(float t)
    {
        return MathF.Pow(2, 10 * (t - 1));
    }

    /// <summary>
    ///     Exponential out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float ExpoOut(float t)
    {
        if (t == 1) return 1;
        return -MathF.Pow(2, -10 * t) + 1;
    }

    /// <summary>
    ///     Exponential in and out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float ExpoInOut(float t)
    {
        if (t == 1) return 1;
        return t < .5f ? MathF.Pow(2, 10 * (t * 2 - 1)) / 2 : (-MathF.Pow(2, -10 * (t * 2 - 1)) + 2) / 2;
    }

    /// <summary>
    ///     Back in.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float BackIn(float t)
    {
        return t * t * (2.70158f * t - 1.70158f);
    }

    /// <summary>
    ///     Back out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float BackOut(float t)
    {
        return 1 - --t * t * (-2.70158f * t - 1.70158f);
    }

    /// <summary>
    ///     Back in and out.
    /// </summary>
    /// <param name="t">Time elapsed.</param>
    /// <returns>Eased timescale.</returns>
    public static float BackInOut(float t)
    {
        t *= 2;
        if (t < 1) return t * t * (2.70158f * t - 1.70158f) / 2;
        t--;
        return (1 - --t * t * (-2.70158f * t - 1.70158f)) / 2 + .5f;
    }
}