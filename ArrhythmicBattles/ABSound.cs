using System.Text.Json.Nodes;
using ArrhythmicBattles.Settings;
using FlexFramework.Core.Audio;

namespace ArrhythmicBattles;

public class ABSound : IDisposable, IConfigurable
{
    public int SfxVolumeLevel
    {
        get => sfxVolumeLevel;
        set
        {
            sfxVolumeLevel = value;
            SelectSfx.Gain = value * 0.1f;
        }
    }
    
    public int MusicVolumeLevel
    {
        get => musicVolumeLevel;
        set
        {
            musicVolumeLevel = value;
            MenuBackgroundMusic.Gain = value * 0.1f;
        }
    }

    private int sfxVolumeLevel = 10;
    private int musicVolumeLevel = 10;
    
    public AudioSource SelectSfx { get; }
    public AudioSource MenuBackgroundMusic { get; }

    private readonly List<AudioStream> audioStreams = new List<AudioStream>();

    public ABSound()
    {
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
        SfxVolumeLevel = (int) jsonObject["sfx"];
        MusicVolumeLevel = (int) jsonObject["music"];
    }
    
    public void Dispose()
    {
        SelectSfx.Dispose();
        MenuBackgroundMusic.Dispose();

        foreach (AudioStream audioStream in audioStreams)
        {
            audioStream.Dispose();
        }
    }
}