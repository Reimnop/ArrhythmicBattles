using ArrhythmicBattles.UI;

namespace ArrhythmicBattles.Util;

public static class Utils
{
    public static void LinkNodesWrapAroundVertical(params NavNode[] nodes)
    {
        if (nodes.Length == 0)
            return;

        nodes[0].Top = nodes[^1];
        nodes[0].Bottom = nodes[1];
        
        nodes[^1].Top = nodes[^2];
        nodes[^1].Bottom = nodes[0];

        for (int i = 1; i < nodes.Length - 1; i++)
        {
            nodes[i].Top = nodes[i - 1];
            nodes[i].Bottom = nodes[i + 1];
        }
    }
    
    public static float RandomFromTime()
    {
        return (float) (DateTime.Now.Millisecond / 1000.0);
    }
}