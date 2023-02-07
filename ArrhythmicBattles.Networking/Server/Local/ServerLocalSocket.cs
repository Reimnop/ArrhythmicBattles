using System.Collections.Concurrent;
using ArrhythmicBattles.Common;
using ArrhythmicBattles.Networking.Client;

namespace ArrhythmicBattles.Networking.Server.Local;

// Emulates network connections between clients and the server
public class ServerLocalSocket : ServerSocket
{
    private readonly Map<ClientLocalSocket, LocalGameClient> clients = new Map<ClientLocalSocket, LocalGameClient>();
    private readonly ConcurrentQueue<ClientLocalSocket> newClients = new ConcurrentQueue<ClientLocalSocket>();
    
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    
    internal void RemoveClient(ClientLocalSocket client)
    {
        clients.Remove(client);
    }
    
    internal void RemoveClient(LocalGameClient gameClient)
    {
        clients.Remove(gameClient);
    }
    
    internal async ValueTask WriteAsync(ClientLocalSocket client, ReadOnlyMemory<byte> buffer)
    {
        LocalGameClient localGameClient = clients.Forward[client];
        await localGameClient.WriteAsync(buffer);
    }
    
    internal async ValueTask WriteAsync(LocalGameClient gameClient, ReadOnlyMemory<byte> buffer)
    {
        ClientLocalSocket localClient = clients.Reverse[gameClient];
        await localClient.WriteAsync(buffer);
    }

    public Task<GameClient> ConnectLocalClientAsync()
    {
        ClientLocalSocket socket = new ClientLocalSocket(this);
        LocalGameClient gameClient = new LocalGameClient(this);
        clients.Add(socket, gameClient);
        newClients.Enqueue(socket);
        return Task.FromResult<GameClient>(gameClient);
    }
    
    public override async Task<ClientSocket?> AcceptAsync()
    {
        ClientLocalSocket? client = null;
        await TaskHelper.WaitUntil(() => newClients.TryDequeue(out client), cancellationToken: cancellationTokenSource.Token);
        return client;
    }

    public override Task DisconnectClientAsync(ClientSocket clientSocket)
    {
        clients.Remove((ClientLocalSocket) clientSocket);
        return Task.CompletedTask;
    }

    public override void Close()
    {
    }
}