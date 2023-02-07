namespace ArrhythmicBattles.Networking.Packets;

public abstract class Packet
{
    public abstract Task<ReadOnlyMemory<byte>> SerializeAsync();
    public abstract Task DeserializeAsync(ReadOnlyMemory<byte> buffer);
}