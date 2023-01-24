namespace ArrhythmicBattles.Networking.Server;

public abstract class ClientSocket
{
    public abstract ValueTask SendAsync(ReadOnlyMemory<byte> buffer);
    public abstract ValueTask<Memory<byte>> ReceiveAsync();
    public abstract void Close();
}