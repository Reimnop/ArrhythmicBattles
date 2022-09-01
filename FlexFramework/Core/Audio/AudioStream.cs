using System.Diagnostics.CodeAnalysis;

namespace FlexFramework.Core.Audio;

public abstract class AudioStream : IDisposable
{
    public abstract float Length { get; }
    public abstract int Channels { get; }
    public abstract int BytesPerSample { get; }
    public abstract int SampleRate { get; }

    public abstract void Restart();
    public abstract bool NextBuffer([MaybeNullWhen(false)] out byte[] data);
    public abstract void Dispose();
}