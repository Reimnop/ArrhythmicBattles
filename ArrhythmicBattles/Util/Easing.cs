namespace ArrhythmicBattles.Util;

public static class Easing
{
    public static double InOutCirc(double x) 
    {
        return x < 0.5
            ? (1.0 - Math.Sqrt(1 - Math.Pow(2.0 * x, 2.0))) / 2.0
            : (Math.Sqrt(1.0 - Math.Pow(-2.0 * x + 2.0, 2.0)) + 1.0) / 2.0;
    }
}