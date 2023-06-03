using FlexFramework.Text;

namespace ArrhythmicBattles.UserInterface;

public static class TextHelper
{
    public static int CalculateTextHeight(Font font, int lines)
    {
        return lines * font.Metrics.Height >> 6;
    }
}