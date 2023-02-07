namespace ArrhythmicBattles.Networking;

public interface ISenderReceiver
{
    ValueTask SendAsync(ReadOnlyMemory<byte> buffer);
    ValueTask<ReadOnlyMemory<byte>> ReceiveAsync();
}