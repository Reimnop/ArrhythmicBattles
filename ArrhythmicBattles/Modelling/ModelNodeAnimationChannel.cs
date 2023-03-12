using ArrhythmicBattles.Core.Animation;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Modelling;

public class ModelNodeAnimationChannel
{
    public string NodeName { get; }
    public IReadOnlyList<Key<Vector3>> PositionKeys => positionKeys;
    public IReadOnlyList<Key<Vector3>> ScaleKeys => scaleKeys;
    public IReadOnlyList<Key<Quaternion>> RotationKeys => rotationKeys;

    private readonly List<Key<Vector3>> positionKeys;
    private readonly List<Key<Vector3>> scaleKeys;
    private readonly List<Key<Quaternion>> rotationKeys;

    public ModelNodeAnimationChannel(string nodeName, List<Key<Vector3>> positionKeys, List<Key<Vector3>> scaleKeys, List<Key<Quaternion>> rotationKeys)
    {
        NodeName = nodeName;
        this.positionKeys = positionKeys;
        this.scaleKeys = scaleKeys;
        this.rotationKeys = rotationKeys;
    }
}