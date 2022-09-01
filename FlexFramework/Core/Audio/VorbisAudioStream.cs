using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using NVorbis;

namespace FlexFramework.Core.Audio;

public class VorbisAudioStream : AudioStream
{
    public override float Length => vorbis.TotalTime.Seconds;
    public override int Channels => vorbis.Channels;
    public override int BytesPerSample => 4;
    public override int SampleRate => vorbis.SampleRate;
    
    private readonly VorbisReader vorbis;
    private readonly float[] readBuffer;

    public VorbisAudioStream(string path)
    {
        vorbis = new VorbisReader(path);
        
        // buffer one second of audio
        readBuffer = new float[vorbis.Channels * vorbis.SampleRate];
    }

    public override void Restart()
    {
        vorbis.SeekTo(0L);
    }

    public override bool NextBuffer([MaybeNullWhen(false)] out byte[] data)
    {
        if (vorbis.ReadSamples(readBuffer) == 0)
        {
            data = null;
            return false;
        }

        data = new byte[readBuffer.Length * sizeof(float)];
        
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        Marshal.Copy(readBuffer, 0, handle.AddrOfPinnedObject(), readBuffer.Length);
        handle.Free();

        return true;
    }

    public override void Dispose()
    {
        vorbis.Dispose();
    }
}