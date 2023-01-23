using System.Diagnostics.CodeAnalysis;

namespace FlexFramework.Core.Audio;

public abstract class AudioStream
{
    public abstract float Length { get; }
    public abstract int Channels { get; }
    public abstract int BytesPerSample { get; }
    public abstract int SampleRate { get; }
    public abstract long SamplePosition { get; }
    public abstract bool Looping { get; set; }

    public abstract void Restart();
    public abstract bool NextBuffer(out Span<byte> data);
}