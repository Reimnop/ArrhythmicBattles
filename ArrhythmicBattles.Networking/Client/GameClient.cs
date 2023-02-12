namespace ArrhythmicBattles.Networking.Client;

public abstract class GameClient : ISenderReceiver
{
    public abstract Task SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    public abstract Task<ReadOnlyMemory<byte>> ReceiveAsync(int length, CancellationToken cancellationToken = default);
    public abstract void Close();
}