namespace ArrhythmicBattles.Networking.Client;

public abstract class GameClient
{
    public abstract ValueTask SendAsync(ReadOnlyMemory<byte> buffer);
    public abstract ValueTask<Memory<byte>> ReceiveAsync();
    public abstract void Close();
}