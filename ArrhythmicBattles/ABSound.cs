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
    
    public StandaloneAudioClip SelectSfx { get; }
    public StandaloneAudioClip MenuBackgroundMusic { get; }

    public ABSound()
    {
        SelectSfx = StandaloneAudioClip.FromWave("Assets/Audio/Select.wav");
        SelectSfx.Looping = false;
        
        MenuBackgroundMusic = StandaloneAudioClip.FromWave("Assets/Audio/Arrhythmic.wav");
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
    }
}