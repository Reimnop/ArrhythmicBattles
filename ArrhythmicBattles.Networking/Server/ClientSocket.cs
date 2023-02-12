namespace ArrhythmicBattles.Networking.Server;

public abstract class ClientSocket : ISenderReceiver
{
    public abstract Task SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    public abstract Task<ReadOnlyMemory<byte>> ReceiveAsync(int length, CancellationToken cancellationToken = default);
    public abstract string GetName();
    public abstract void Close();
}