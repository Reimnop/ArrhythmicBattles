namespace ArrhythmicBattles.Networking.Client;

public abstract class GameClient : ISenderReceiver
{
    public abstract ValueTask SendAsync(ReadOnlyMemory<byte> buffer);
    public abstract ValueTask<ReadOnlyMemory<byte>> ReceiveAsync(int length);
    public abstract void Close();
}