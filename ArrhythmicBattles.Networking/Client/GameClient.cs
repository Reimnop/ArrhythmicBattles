namespace ArrhythmicBattles.Networking.Client;

public abstract class GameClient
{
    public abstract ValueTask SendAsync(ReadOnlyMemory<byte> buffer);
    public abstract ValueTask<ReadOnlyMemory<byte>> ReceiveAsync();
    public abstract void Close();
}