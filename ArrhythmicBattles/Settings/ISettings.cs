using System.ComponentModel;
using Config.Net;

namespace ArrhythmicBattles.Settings;

public interface ISettings : INotifyPropertyChanged
{
    [Option(DefaultValue = 1.0f)]
    float MusicVolume { get; set; }

    [Option(DefaultValue = 1.0f)]
    float SfxVolume { get; set; }
}