using Glide;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UserInterface.Animation;

public class Color4Lerper : MemberLerper
{
    private Color4 from;
    private Color4 to;
    
    public override void Initialize(object fromValue, object toValue, Behavior behavior)
    {
        from = (Color4) fromValue;
        to = (Color4) toValue;
    }

    public override object Interpolate(float t, object currentValue, Behavior behavior)
    {
        return new Color4(
            MathHelper.Lerp(from.R, to.R, t),
            MathHelper.Lerp(from.G, to.G, t),
            MathHelper.Lerp(from.B, to.B, t),
            MathHelper.Lerp(from.A, to.A, t));
    }
}