using OpenTK.Audio.OpenAL;

namespace FlexFramework.Core.Audio;

public class AudioClip : IDisposable
{
    public int Handle { get; }
    public int SampleRate { get; }
    public int SizeInBytes { get; }

    public AudioClip(ALFormat format, byte[] data, int sampleRate, int sizeInBytes = -1)
    {
        SizeInBytes = data.Length;
        SampleRate = sampleRate;

        int size = sizeInBytes < 0 ? data.Length : sizeInBytes;
        
        Handle = AL.GenBuffer();
        AL.BufferData(Handle, format, ref data[0], size, sampleRate);
    }

    public static AudioClip FromWave(string path)
    {
        using FileStream stream = File.OpenRead(path);
        byte[] buffer = LoadWave(stream, out int channels, out int bits, out int sampleRate);
        return new AudioClip(GetSoundFormat(channels, bits), buffer, sampleRate, buffer.Length - buffer.Length % (bits / 8 * channels));
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

    public void Dispose()
    {
        AL.DeleteBuffer(Handle);
    }
}