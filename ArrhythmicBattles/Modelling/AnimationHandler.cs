using ArrhythmicBattles.Core.Animation;
using ArrhythmicBattles.Modelling;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Modelling;

public class AnimationHandler
{
    private class NodeSequenceCollection
    {
        public Sequence<Vector3> PositionSequence { get; }
        public Sequence<Vector3> ScaleSequence { get; }
        public Sequence<Quaternion> RotationSequence { get; }

        public Matrix4 Transform { get; private set; } = Matrix4.Identity;

        public NodeSequenceCollection(ModelNodeAnimationChannel nodeAnimationChannel)
        {
            Interpolator<Vector3> vec3Interpolator = (first, second, factor) => new Vector3(
                MathHelper.Lerp(first.X, second.X, factor),
                MathHelper.Lerp(first.Y, second.Y, factor),
                MathHelper.Lerp(first.Z, second.Z, factor));

            PositionSequence = new Sequence<Vector3>(nodeAnimationChannel.PositionKeys, vec3Interpolator);
            ScaleSequence = new Sequence<Vector3>(nodeAnimationChannel.ScaleKeys, vec3Interpolator);
            RotationSequence = new Sequence<Quaternion>(nodeAnimationChannel.RotationKeys, Quaternion.Slerp);
        }

        public void Update(float time)
        {
            PositionSequence.Interpolate(time);
            ScaleSequence.Interpolate(time);
            RotationSequence.Interpolate(time);

            Transform = CalculateTransform();
        }

        private Matrix4 CalculateTransform()
        {
            return Matrix4.CreateScale(ScaleSequence.CurrentValue) *
                   Matrix4.CreateFromQuaternion(RotationSequence.CurrentValue) *
                   Matrix4.CreateTranslation(PositionSequence.CurrentValue);
        }
    }
    
    private readonly Dictionary<string, NodeSequenceCollection> nodeNameToAnim;

    public AnimationHandler(ModelAnimation animation)
    {
        nodeNameToAnim = new Dictionary<string, NodeSequenceCollection>();
        
        foreach (ModelNodeAnimationChannel nodeAnimationChannel in animation.NodeAnimationChannels)
        {
            nodeNameToAnim.Add(nodeAnimationChannel.NodeName, new NodeSequenceCollection(nodeAnimationChannel));
        }
    }

    public void Update(float time)
    {
        foreach (var (_, anim) in nodeNameToAnim)
        {
            anim.Update(time);
        }
    }

    public Matrix4 GetNodeTransform(ModelNode node)
    {
        if (nodeNameToAnim.TryGetValue(node.Name, out NodeSequenceCollection? anim))
        {
            return anim.Transform;
        }

        return node.Transform;
    }
}