namespace ArrhythmicBattles.Networking.Packets;

public class AuthPacket : Packet
{
    public string Username { get; set; } = string.Empty;
    public long Id { get; set; } = 0;

    public AuthPacket()
    {
    }
    
    public AuthPacket(string username, long id)
    {
        Username = username;
        Id = id;
    }

    public override Task<ReadOnlyMemory<byte>> SerializeAsync()
    {
        using MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(Username);
        writer.Write(Id);
        
        return Task.FromResult((ReadOnlyMemory<byte>) stream.ToArray());
    }

    public override Task DeserializeAsync(ReadOnlyMemory<byte> buffer)
    {
        using MemoryStream stream = new MemoryStream(buffer.ToArray());
        BinaryReader reader = new BinaryReader(stream);
        Username = reader.ReadString();
        Id = reader.ReadInt64();
        
        return Task.CompletedTask;
    }
}