namespace ArrhythmicBattles.Networking.Packets;

public class AuthPacket : Packet
{
    public string Username { get; set; } = string.Empty;

    public AuthPacket()
    {
    }
    
    public AuthPacket(string username)
    {
        Username = username;
    }

    public override Task<ReadOnlyMemory<byte>> SerializeAsync()
    {
        using MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(Username);
        
        return Task.FromResult((ReadOnlyMemory<byte>) stream.ToArray());
    }

    public override Task DeserializeAsync(ReadOnlyMemory<byte> buffer)
    {
        using MemoryStream stream = new MemoryStream(buffer.ToArray());
        BinaryReader reader = new BinaryReader(stream);
        Username = reader.ReadString();
        
        return Task.CompletedTask;
    }
}