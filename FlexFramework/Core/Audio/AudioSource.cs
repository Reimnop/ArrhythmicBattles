using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Audio;

public class AudioSource : IDisposable
{
    public int Handle { get; }
    public float Gain
    {
        get => gain;
        set
        {
            gain = value;
            AL.Source(Handle, ALSourcef.Gain, gain);
        }
    }
    public float Pitch
    {
        get => pitch;
        set
        {
            pitch = value;
            AL.Source(Handle, ALSourcef.Pitch, pitch);
        }
    }
    public bool Playing => AL.GetSourceState(Handle) == ALSourceState.Playing;
    public Vector3 Position
    {
        get
        {
            AL.GetSource(Handle, ALSource3f.Position, out Vector3 position);
            return position;
        }
        set
        {
            Vector3 position = value;
            AL.Source(Handle, ALSource3f.Position, ref position);
        }
    }
    
    private float gain = 1.0f;
    private float pitch = 1.0f;

    private readonly Thread audioThread;
    private bool shouldUpdate = true;

    public AudioStream? AudioStream
    {
        get => audioStream;
        set
        {
            audioStream = value;
            CleanAllBuffers();
        }
    }

    private AudioStream? audioStream;

    public AudioSource()
    {
        Handle = AL.GenSource();
        AL.Source(Handle, ALSourcef.Gain, gain);
        AL.Source(Handle, ALSourcef.Pitch, pitch);
        AL.Source(Handle, ALSourcei.SourceType, (int) ALSourceType.Streaming);

        audioThread = new Thread(UpdateLoop);
        audioThread.IsBackground = true;
        audioThread.Start();
    }

    private void UpdateLoop()
    {
        while (shouldUpdate)
        {
            Update();
            Thread.Sleep(4);
        }
    }

    private void Update()
    {
        if (audioStream == null)
        {
            return;
        }
        
        lock (audioStream)
        {
            if (!Playing)
            {
                return;
            }

            QueueBuffers(audioStream);
        }
    }

    private void QueueBuffers(AudioStream stream, int numBuffers = 2)
    {
        AL.GetSource(Handle, ALGetSourcei.BuffersProcessed, out int buffersProcessed);

        // Delete all used buffers
        if (buffersProcessed > 0) 
        {
            Span<int> buffersToDelete = stackalloc int[buffersProcessed];
            AL.SourceUnqueueBuffers(Handle, buffersToDelete);
            AL.DeleteBuffers(buffersToDelete);
        }

        // Queue new buffers
        AL.GetSource(Handle, ALGetSourcei.BuffersQueued, out int buffersQueued);
        int buffersToQueue = numBuffers - buffersQueued;
        for (int i = 0; i < buffersToQueue; i++)
        {
            if (!stream.NextBuffer(out Span<byte> data))
            {
                break;
            }

            int buffer = AL.GenBuffer();
            AL.BufferData<byte>(buffer, DetermineSoundFormat(stream.BytesPerSample, stream.Channels), data, stream.SampleRate);
            AL.SourceQueueBuffer(Handle, buffer);
        }
    }

    private static ALFormat DetermineSoundFormat(int bytesPerSample, int channels)
    {
        return (bytesPerSample, channels) switch
        {
            (1, 1) => ALFormat.Mono8,
            (1, 2) => ALFormat.Stereo8,
            (2, 1) => ALFormat.Mono16,
            (2, 2) => ALFormat.Stereo16,
            (4, 1) => ALFormat.MonoFloat32Ext,
            (4, 2) => ALFormat.StereoFloat32Ext,
            (_, _) => throw new NotImplementedException()
        };
    }
    
    private void CleanAllBuffers()
    {
        AL.GetSource(Handle, ALGetSourcei.BuffersQueued, out int buffersQueued);

        if (buffersQueued > 0)
        {
            Span<int> buffers = stackalloc int[buffersQueued];
            AL.SourceUnqueueBuffers(buffersQueued, buffers);
            AL.DeleteBuffers(buffers);
        }
    }

    public void Play()
    {
        if (audioStream == null)
        {
            return;
        }
        
        AL.SourceStop(Handle);

        lock (audioStream)
        {
            if (audioStream.SamplePosition > 0)
            {
                audioStream.Restart();
            }
        
            CleanAllBuffers();
            QueueBuffers(audioStream);
        }
        
        AL.SourcePlay(Handle);
    }

    public void Resume()
    {
        AL.SourcePlay(Handle);
    }

    public void Pause()
    {
        AL.SourcePause(Handle);
    }

    public void Stop()
    {
        AL.SourceStop(Handle);
    }

    public void Dispose()
    {
        AL.SourceStop(Handle);
        
        shouldUpdate = false;
        CleanAllBuffers();

        AL.DeleteSource(Handle);
    }
}