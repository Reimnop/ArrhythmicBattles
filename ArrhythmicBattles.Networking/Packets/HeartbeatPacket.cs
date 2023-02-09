namespace ArrhythmicBattles.Networking.Packets;

public class HeartbeatPacket : Packet
{
    public HeartbeatPacket()
    {
    }
    
    public override Task<ReadOnlyMemory<byte>> SerializeAsync()
    {
        return Task.FromResult(ReadOnlyMemory<byte>.Empty);
    }

    public override Task DeserializeAsync(ReadOnlyMemory<byte> buffer)
    {
        return Task.CompletedTask;
    }
}