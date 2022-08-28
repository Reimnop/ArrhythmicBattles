namespace ArrhythmicBattles.Modelling.Animate;

public class ModelAnimation
{
    public string Name { get; }
    public float DurationInTicks { get; }
    public float TicksPerSecond { get; }

    public IReadOnlyList<ModelNodeAnimationChannel> NodeAnimationChannels => nodeAnimationChannels;

    private readonly List<ModelNodeAnimationChannel> nodeAnimationChannels;

    public ModelAnimation(string name, float durationInTicks, float ticksPerSecond, List<ModelNodeAnimationChannel> nodeAnimationChannels)
    {
        Name = name;
        DurationInTicks = durationInTicks;
        TicksPerSecond = ticksPerSecond;
        this.nodeAnimationChannels = nodeAnimationChannels;
    }
}