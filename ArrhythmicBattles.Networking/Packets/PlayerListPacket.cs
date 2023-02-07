using ArrhythmicBattles.Common;

namespace ArrhythmicBattles.Networking.Packets;

public class PlayerListPacket : Packet
{
    public List<PlayerProfile> Players { get; set; }

    public PlayerListPacket()
    {
        Players = new List<PlayerProfile>();
    }
    
    public PlayerListPacket(List<PlayerProfile> players)
    {
        Players = players;
    }
    
    public override Task<ReadOnlyMemory<byte>> SerializeAsync()
    {
        using MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(Players.Count);
        foreach (PlayerProfile player in Players)
        {
            writer.Write(player.Username);
            writer.Write(player.Id);
        }
        return Task.FromResult((ReadOnlyMemory<byte>) stream.ToArray());
    }

    public override Task DeserializeAsync(ReadOnlyMemory<byte> buffer)
    {
        using MemoryStream stream = new MemoryStream(buffer.ToArray());
        BinaryReader reader = new BinaryReader(stream);
        int count = reader.ReadInt32();
        Players = new List<PlayerProfile>();
        for (int i = 0; i < count; i++)
        {
            Players.Add(new PlayerProfile(reader.ReadString(), reader.ReadInt32()));
        }
        return Task.CompletedTask;
    }
}