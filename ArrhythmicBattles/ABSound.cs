using FlexFramework.Core.Audio;

namespace ArrhythmicBattles;

public class ABSound : IDisposable
{
    public StandaloneAudioClip SelectSfx { get; }
    public StandaloneAudioClip MenuBackgroundMusic { get; }

    public ABSound()
    {
        SelectSfx = StandaloneAudioClip.FromWave("Assets/Audio/Select.wav");
        SelectSfx.Looping = false;
        
        MenuBackgroundMusic = StandaloneAudioClip.FromWave("Assets/Audio/Arrhythmic.wav");
    }

    public void Dispose()
    {
        SelectSfx.Dispose();
        MenuBackgroundMusic.Dispose();
    }
}