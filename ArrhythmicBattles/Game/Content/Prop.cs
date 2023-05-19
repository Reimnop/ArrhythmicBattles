using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content;

public abstract class Prop
{
    public abstract Vector3 Position { get; set; }
    public abstract Vector3 Scale { get; set; }
    public abstract Quaternion Rotation { get; set; }
}