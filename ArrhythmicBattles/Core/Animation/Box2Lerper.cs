using Glide;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Core.Animation;

public class Box2Lerper : MemberLerper
{
    private Box2 from;
    private Box2 to;
    
    public override void Initialize(object fromValue, object toValue, Behavior behavior)
    {
        from = (Box2) fromValue;
        to = (Box2) toValue;
    }

    public override object Interpolate(float t, object currentValue, Behavior behavior)
    {
        return new Box2(
            MathHelper.Lerp(from.Min.X, to.Min.X, t),
            MathHelper.Lerp(from.Min.Y, to.Min.Y, t),
            MathHelper.Lerp(from.Max.X, to.Max.X, t),
            MathHelper.Lerp(from.Max.Y, to.Max.Y, t));
    }
}