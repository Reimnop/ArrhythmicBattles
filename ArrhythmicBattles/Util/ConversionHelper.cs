using OpenTK.Mathematics;

using SysVector3 = System.Numerics.Vector3;
using SysQuaternion = System.Numerics.Quaternion;

namespace ArrhythmicBattles.Util;

public static class ConversionHelper
{
    public static Vector3 ToOpenTK(this SysVector3 vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }
    
    public static SysVector3 ToSystem(this Vector3 vector)
    {
        return new SysVector3(vector.X, vector.Y, vector.Z);
    }
    
    public static Quaternion ToOpenTK(this SysQuaternion quaternion)
    {
        return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
    }
    
    public static SysQuaternion ToSystem(this Quaternion quaternion)
    {
        return new SysQuaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
    }
}