namespace ArrhythmicBattles.Util;

public static class RandomHelper
{
    public static float RandomFromTime()
    {
        return (float) (DateTime.Now.Millisecond / 1000.0);
    }
}