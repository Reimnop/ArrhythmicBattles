using OpenTK.Mathematics;

namespace ArrhythmicBattles.Util;

public static class ColorUtil
{
    public static Color4 ParseHex(string hex)
    {
        // Remove the hash if it exists
        if (hex.StartsWith("#"))
            hex = hex.Substring(1);

        // Convert to rgb bytes
        var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        
        // Check if alpha channel is present
        byte a = hex.Length == 8
            ? byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
            : byte.MaxValue;

        return new Color4(r, g, b, a);
    }
}