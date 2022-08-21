namespace ArrhythmicBattles.Util;

// Copy pasted code from my other project
public static class Easing
{
	private const double PI = 3.14159265359;
	private const double PI2 = PI / 2;
	private const double B1 = 1 / 2.75;
	private const double B2 = 2 / 2.75;
	private const double B3 = 1.5 / 2.75;
	private const double B4 = 2.5 / 2.75;
	private const double B5 = 2.25 / 2.75;
	private const double B6 = 2.625 / 2.75;

    /// <summary>
	/// Linear.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double Linear(double t)
		=> t;

	/// <summary>
	/// Instant.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double Instant(double t)
	{
		if (t == 1.0f) return 1.0f;
		return 0.0f;
	}

	#region Sine

	/// <summary>
	/// Sine in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double SineIn(double t)
	{
		if (t == 1) return 1;
		return -Math.Cos(PI2 * t) + 1;
	}

	/// <summary>
	/// Sine out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double SineOut(double t)
	{
		return Math.Sin(PI2 * t);
	}

	/// <summary>
	/// Sine in and out
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double SineInOut(double t)
	{
		return -Math.Cos(PI * t) / 2 + 0.5f;
	}

	#endregion

	#region Elastic

	/// <summary>
	/// Elastic in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double ElasticIn(double t)
	{
		return (Math.Sin(13 * PI2 * t) * Math.Pow(2, 10 * (t - 1)));
	}

	/// <summary>
	/// Elastic out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double ElasticOut(double t)
	{
		if (t == 1) return 1;
		return (Math.Sin(-13 * PI2 * (t + 1)) * Math.Pow(2, -10 * t) + 1);
	}

	/// <summary>
	/// Elastic in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double ElasticInOut(double t)
	{
		if (t < 0.5)
		{
			return (0.5f * Math.Sin(13 * PI2 * (2 * t)) * Math.Pow(2, 10 * ((2 * t) - 1)));
		}

		return (0.5f * (Math.Sin(-13 * PI2 * ((2 * t - 1) + 1)) * Math.Pow(2, -10 * (2 * t - 1)) + 2));
	}

	#endregion

	#region Back

	/// <summary>
	/// Back in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double BackIn(double t)
	{
		return (t * t * (2.70158f * t - 1.70158f));
	}

	/// <summary>
	/// Back out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double BackOut(double t)
	{
		return (1 - (--t) * (t) * (-2.70158f * t - 1.70158f));
	}

	/// <summary>
	/// Back in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double BackInOut(double t)
	{
		t *= 2;
		if (t < 1) return (t * t * (2.70158f * t - 1.70158f) / 2);
		t--;
		return ((1 - (--t) * (t) * (-2.70158f * t - 1.70158f)) / 2 + .5f);
	}

	#endregion

	#region Bounce

	/// <summary>
	/// Bounce in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double BounceIn(double t)
	{
		t = 1 - t;
		if (t < B1) return (1 - 7.5625f * t * t);
		if (t < B2) return (1 - (7.5625f * (t - B3) * (t - B3) + .75f));
		if (t < B4) return (1 - (7.5625f * (t - B5) * (t - B5) + .9375f));
		return (1 - (7.5625f * (t - B6) * (t - B6) + .984375f));
	}

	/// <summary>
	/// Bounce out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double BounceOut(double t)
	{
		if (t < B1) return (7.5625f * t * t);
		if (t < B2) return (7.5625f * (t - B3) * (t - B3) + .75f);
		if (t < B4) return (7.5625f * (t - B5) * (t - B5) + .9375f);
		return (7.5625f * (t - B6) * (t - B6) + .984375f);
	}

	/// <summary>
	/// Bounce in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double BounceInOut(double t)
	{
		if (t < .5)
		{
			t = 1 - t * 2;
			if (t < B1) return ((1 - 7.5625f * t * t) / 2);
			if (t < B2) return ((1 - (7.5625f * (t - B3) * (t - B3) + .75f)) / 2);
			if (t < B4) return ((1 - (7.5625f * (t - B5) * (t - B5) + .9375f)) / 2);
			return ((1 - (7.5625f * (t - B6) * (t - B6) + .984375f)) / 2);
		}

		t = t * 2 - 1;
		if (t < B1) return ((7.5625f * t * t) / 2 + .5f);
		if (t < B2) return ((7.5625f * (t - B3) * (t - B3) + .75f) / 2 + .5f);
		if (t < B4) return ((7.5625f * (t - B5) * (t - B5) + .9375f) / 2 + .5f);
		return ((7.5625f * (t - B6) * (t - B6) + .984375f) / 2 + .5f);
	}

	#endregion

	#region Quad

	/// <summary>
	/// Quadratic in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double QuadIn(double t)
	{
		return (t * t);
	}

	/// <summary>
	/// Quadratic out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double QuadOut(double t)
	{
		return (-t * (t - 2));
	}

	/// <summary>
	/// Quadratic in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double QuadInOut(double t)
	{
		return (t <= .5 ? t * t * 2 : 1 - (--t) * t * 2);
	}

	#endregion

	#region Circ

	/// <summary>
	/// Circle in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double CircIn(double t)
	{
		return (-(Math.Sqrt(1 - t * t) - 1));
	}

	/// <summary>
	/// Circle out
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double CircOut(double t)
	{
		return (Math.Sqrt(1 - (t - 1) * (t - 1)));
	}

	/// <summary>
	/// Circle in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double CircInOut(double t)
	{
		return (t <= .5 ? (Math.Sqrt(1 - t * t * 4) - 1) / -2 : (Math.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) / 2);
	}

	#endregion

	#region Expo

	/// <summary>
	/// Exponential in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double ExpoIn(double t)
	{
		return (Math.Pow(2, 10 * (t - 1)));
	}

	/// <summary>
	/// Exponential out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double ExpoOut(double t)
	{
		if (t == 1) return 1;
		return (-Math.Pow(2, -10 * t) + 1);
	}

	/// <summary>
	/// Exponential in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static double ExpoInOut(double t)
	{
		if (t == 1) return 1;
		return (t < .5 ? Math.Pow(2, 10 * (t * 2 - 1)) / 2 : (-Math.Pow(2, -10 * (t * 2 - 1)) + 2) / 2);
	}

	#endregion
}