using ArrhythmicBattles.Common;

namespace ArrhythmicBattles.Networking;

public class ByteReceiver : IDisposable
{
    private readonly ISenderReceiver senderReceiver;
    private readonly ByteQueue byteQueue = new ByteQueue();
    
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    
    public ByteReceiver(ISenderReceiver senderReceiver)
    {
        this.senderReceiver = senderReceiver;
        Task.Run(ReceiveAsync);
    }
    
    private async Task ReceiveAsync()
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            ReadOnlyMemory<byte> buffer = await senderReceiver.ReceiveAsync();
            byteQueue.Enqueue(buffer.ToArray(), 0, buffer.Length);
        }
    }
    
    public async Task<ReadOnlyMemory<byte>> NextBytesAsync(int length)
    {
        await TaskHelper.WaitUntil(() => byteQueue.Length >= length, cancellationToken: cancellationTokenSource.Token);
        
        byte[] bytes = new byte[length];
        byteQueue.Dequeue(bytes, 0, length);
        return bytes;
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}