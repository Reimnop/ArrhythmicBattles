using System.Runtime.InteropServices;
using NVorbis;

namespace FlexFramework.Core.Audio;

public class VorbisAudioStream : AudioStream
{
    public override float Length => vorbis.TotalTime.Seconds;
    public override int Channels => vorbis.Channels;
    public override int BytesPerSample => 4;
    public override int SampleRate => vorbis.SampleRate;
    public override long SamplePosition => vorbis.SamplePosition;
    public override bool Looping { get; set; }

    private readonly VorbisReader vorbis;
    private readonly float[] readBuffer;
    private readonly byte[] copyBuffer;

    public VorbisAudioStream(string path)
    {
        vorbis = new VorbisReader(path);
        
        // buffer one second of audio
        readBuffer = new float[vorbis.Channels * vorbis.SampleRate];
        copyBuffer = new byte[vorbis.Channels * vorbis.SampleRate * sizeof(float)];
    }

    public override void Restart()
    {
        vorbis.SeekTo(0L);
    }

    private int ReadSamples(Span<float> buffer)
    {
        int readLength = vorbis.ReadSamples(buffer);
        
        if (readLength == 0 && Looping)
        {
            vorbis.SeekTo(0L);
            return vorbis.ReadSamples(buffer);
        }

        return readLength;
    }

    public override bool NextBuffer(out Span<byte> data)
    {
        int readLength = ReadSamples(readBuffer);
        
        if (readLength == 0)
        {
            data = null;
            return false;
        }

        GCHandle handle = GCHandle.Alloc(copyBuffer, GCHandleType.Pinned);
        Marshal.Copy(readBuffer, 0, handle.AddrOfPinnedObject(), readLength);
        handle.Free();

        data = new Span<byte>(copyBuffer, 0, readLength * sizeof(float));
        return true;
    }

    public override void Dispose()
    {
        vorbis.Dispose();
    }
}