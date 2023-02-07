namespace ArrhythmicBattles.Common;

public struct PlayerProfile
{
    public string Username { get; set; } = string.Empty;
    public long Id { get; set; } = 0;
    
    public PlayerProfile(string username, long id)
    {
        Username = username;
        Id = id;
    }
}