using ArrhythmicBattles.Common;
using ArrhythmicBattles.Networking.Server;

namespace ArrhythmicBattles.Server;

public class Player : IDisposable
{
    private readonly ClientSocket client;
    private readonly ByteQueue receiveQueue = new ByteQueue();
    
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    internal Player(ClientSocket client)
    {
        this.client = client;
        Task.Run(ReceiveAsync);
    }

    private async Task ReceiveAsync()
    {
        ReadOnlyMemory<byte> buffer;
        do
        {
            buffer = await client.ReceiveAsync();
            byte[] data = buffer.ToArray();
            receiveQueue.Enqueue(data, 0, data.Length);
        }
        while (buffer.Length > 0 && !cancellationTokenSource.IsCancellationRequested);
    }

    private async Task<ReadOnlyMemory<byte>> NextBytesAsync(int length)
    {
        await TaskHelper.WaitUntil(() => receiveQueue.Length >= length, cancellationToken: cancellationTokenSource.Token);
        byte[] buffer = new byte[length];
        receiveQueue.Dequeue(buffer, 0, buffer.Length);
        return buffer;
    }

    public async Task<ReadOnlyMemory<byte>> ReceivePacketAsync()
    {
        ReadOnlyMemory<byte> lengthBuffer = await NextBytesAsync(4);
        int length = BitConverter.ToInt32(lengthBuffer.Span);
        return await NextBytesAsync(length);
    }
    
    public async Task SendPacketAsync(ReadOnlyMemory<byte> packet)
    {
        using MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(packet.Length);
        writer.Write(packet.Span);
        await client.SendAsync(stream.ToArray());
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        client.Close();
    }
}