using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Audio;

public class StandaloneAudioClip
{
    public int ClipHandle { get; }
    public int SourceHandle { get; }
    
    public int SampleRate { get; }
    public int SizeInBytes { get; }

    private float gain = 1.0f;
    private float pitch = 1.0f;
    private bool loop = true;

    public double Gain
    {
        get => gain;
        set
        {
            gain = (float) value;
            AL.Source(SourceHandle, ALSourcef.Gain, gain);
        }
    }

    public double Pitch
    {
        get => pitch;
        set
        {
            pitch = (float) value;
            AL.Source(SourceHandle, ALSourcef.Pitch, pitch);
        }
    }

    public bool Looping
    {
        get => loop;
        set
        {
            if (loop == value)
            {
                return;
            }

            loop = value;
            AL.Source(SourceHandle, ALSourceb.Looping, value);
        }
    }

    public bool Playing => AL.GetSourceState(SourceHandle) == ALSourceState.Playing;

    public Vector3d Position
    {
        get
        {
            AL.GetSource(SourceHandle, ALSource3f.Position, out Vector3 position);
            return position;
        }
        set
        {
            Vector3 position = (Vector3) value;
            AL.Source(SourceHandle, ALSource3f.Position, ref position);
        }
    }
    
    public int SamplePosition
    {
        get
        {
            AL.GetSource(SourceHandle, ALGetSourcei.SampleOffset, out int bPosition);
            return bPosition;
        }
    }

    public double PlayPosition => SamplePosition / (double) SampleRate;

    public StandaloneAudioClip(ALFormat format, byte[] data, int sampleRate, int sizeInBytes = -1)
    {
        SizeInBytes = data.Length;
        SampleRate = sampleRate;

        int size = sizeInBytes < 0 ? data.Length : sizeInBytes;
        
        ClipHandle = AL.GenBuffer();
        AL.BufferData(ClipHandle, format, ref data[0], size, sampleRate);
        
        SourceHandle = AL.GenSource();
        AL.Source(SourceHandle, ALSourcef.Gain, gain);
        AL.Source(SourceHandle, ALSourcef.Pitch, pitch);
        AL.Source(SourceHandle, ALSourceb.Looping, loop);
        AL.Source(SourceHandle, ALSourcei.Buffer, ClipHandle);
    }

    public static StandaloneAudioClip FromWave(string path)
    {
        using FileStream stream = File.OpenRead(path);
        byte[] buffer = LoadWave(stream, out int channels, out int bits, out int sampleRate);
        return new StandaloneAudioClip(GetSoundFormat(channels, bits), buffer, sampleRate, buffer.Length - buffer.Length % (bits / 8 * channels));
    }

    private static byte[] LoadWave(Stream stream, out int channels, out int bits, out int sampleRate)
    {
        using BinaryReader reader = new BinaryReader(stream);
        
        // RIFF header
        string signature = new string(reader.ReadChars(4));
        
        if (signature != "RIFF")
        {
            throw new NotSupportedException("Specified stream is not a wave file");
        }

        int riffChunkSize = reader.ReadInt32();

        string format = new string(reader.ReadChars(4));
        if (format != "WAVE")
        {
            throw new NotSupportedException("Specified stream is not a wave file");
        }

        // WAVE header
        string formatSignature = new string(reader.ReadChars(4));
        if (formatSignature != "fmt ")
        {
            throw new NotSupportedException("Specified wave file is not supported");
        }

        int formatChunkSize = reader.ReadInt32();
        int audioFormat = reader.ReadInt16();
        int numChannels = reader.ReadInt16();
        int rate = reader.ReadInt32();
        int byteRate = reader.ReadInt32();
        int blockAlign = reader.ReadInt16();
        int bitsPerSample = reader.ReadInt16();

        string dataSignature = new string(reader.ReadChars(4));
        if (dataSignature != "data")
        {
            throw new NotSupportedException("Specified wave file is not supported.");
        }

        int dataChunkSize = reader.ReadInt32();

        channels = numChannels;
        bits = bitsPerSample;
        sampleRate = rate;

        return reader.ReadBytes((int) reader.BaseStream.Length);
    }

    private static ALFormat GetSoundFormat(int channels, int bits)
    {
        return (channels, bits) switch
        {
            (1, 8) => ALFormat.Mono8,
            (1, 16) => ALFormat.Mono16,
            (2, 8) => ALFormat.Stereo8,
            (2, 16) => ALFormat.Stereo16,
            _ => throw new NotSupportedException("The specified sound format is not supported")
        };
    }
    
    public void Play()
    {
        AL.SourcePlay(SourceHandle);
    }

    public void Pause()
    {
        AL.SourcePause(SourceHandle);
    }

    public void Stop()
    {
        AL.SourceStop(SourceHandle);
    }

    public void Dispose()
    {
        AL.DeleteBuffer(ClipHandle);
        AL.DeleteSource(SourceHandle);
    }
}