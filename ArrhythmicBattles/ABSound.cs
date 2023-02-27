using System.Text.Json.Nodes;
using ArrhythmicBattles.Settings;
using FlexFramework.Core.Audio;

namespace ArrhythmicBattles;

public class ABSound : IDisposable, IConfigurable
{
    public float SfxVolumeLevel
    {
        get => SelectSfx.Gain;
        set
        {
            SelectSfx.Gain = value;
            context.SaveSettings();
        }
    }
    
    public float MusicVolumeLevel
    {
        get => MenuBackgroundMusic.Gain;
        set
        {
            MenuBackgroundMusic.Gain = value;
            context.SaveSettings();
        }
    }

    public AudioSource SelectSfx { get; }
    public AudioSource MenuBackgroundMusic { get; }

    private readonly ABContext context;
    private readonly List<AudioStream> audioStreams = new List<AudioStream>();

    public ABSound(ABContext context)
    {
        this.context = context;
        
        MenuBackgroundMusic = InitAudioSource("Assets/Audio/Arrhythmic.ogg", true);
        SelectSfx = InitAudioSource("Assets/Audio/Select.ogg", false);
    }

    private AudioSource InitAudioSource(string path, bool looping)
    {
        VorbisAudioStream audioStream = new VorbisAudioStream(path);
        audioStream.Looping = looping;
        
        AudioSource audioSource = new AudioSource();
        audioSource.AudioStream = audioStream;
        audioStreams.Add(audioStream);

        return audioSource;
    }

    public JsonObject ToJson()
    {
        JsonObject jsonObject = new JsonObject();
        jsonObject["sfx"] = SfxVolumeLevel;
        jsonObject["music"] = MusicVolumeLevel;
        
        return jsonObject;
    }

    public void FromJson(JsonObject jsonObject)
    {
        SelectSfx.Gain = (float) jsonObject["sfx"];
        MenuBackgroundMusic.Gain = (float) jsonObject["music"];
    }
    
    public void Dispose()
    {
        SelectSfx.Dispose();
        MenuBackgroundMusic.Dispose();

        foreach (IDisposable disposable in audioStreams.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}