namespace ArrhythmicBattles.Networking.Server;

public abstract class ClientSocket : ISenderReceiver
{
    public abstract ValueTask SendAsync(ReadOnlyMemory<byte> buffer);
    public abstract ValueTask<ReadOnlyMemory<byte>> ReceiveAsync();
    public abstract void Close();
}