namespace ArrhythmicBattles.Networking;

public interface ISenderReceiver
{
    ValueTask SendAsync(ReadOnlyMemory<byte> buffer);
    ValueTask<ReadOnlyMemory<byte>> ReceiveAsync(int length = -1); // Should return empty memory if client is disconnected, else wait for data
}