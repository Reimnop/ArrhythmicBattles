using Glide;
using OpenTK.Mathematics;

namespace FlexFramework.Modelling.Animation;

public class Vector2Lerper : MemberLerper
{
    private Vector2 from;
    private Vector2 to;
    
    public override void Initialize(object fromValue, object toValue, Behavior behavior)
    {
        from = (Vector2) fromValue;
        to = (Vector2) toValue;
    }

    public override object Interpolate(float t, object currentValue, Behavior behavior)
    {
        return Vector2.Lerp(from, to, t);
    }
}