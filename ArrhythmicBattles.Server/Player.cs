using ArrhythmicBattles.Common;
using ArrhythmicBattles.Networking.Client;

namespace ArrhythmicBattles.Server;

public class Player : IDisposable
{
    private readonly GameClient client;
    private readonly Task receiveTask;
    
    private readonly ByteQueue receiveQueue = new ByteQueue();

    internal Player(GameClient client)
    {
        this.client = client;
        receiveTask = Task.Run(Receive);
    }

    private async Task Receive()
    {
        ReadOnlyMemory<byte> buffer = default;
        do
        {
            buffer = await client.ReceiveAsync();
            byte[] data = buffer.ToArray();
            receiveQueue.Enqueue(data, 0, data.Length);
        }
        while (buffer.Length > 0);
    }

    public void Dispose()
    {
        client.Close();
        receiveTask.Dispose();
    }
}