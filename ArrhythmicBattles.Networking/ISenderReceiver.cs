namespace ArrhythmicBattles.Networking;

public interface ISenderReceiver
{
    Task SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    Task<ReadOnlyMemory<byte>> ReceiveAsync(int length = -1, CancellationToken cancellationToken = default); // Should return empty memory if client is disconnected, else wait for data
}