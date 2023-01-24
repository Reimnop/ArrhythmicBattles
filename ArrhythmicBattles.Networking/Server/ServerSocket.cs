namespace ArrhythmicBattles.Networking.Server;

public abstract class ServerSocket
{
    public abstract Task<ClientSocket> AcceptAsync();
    public abstract Task DisconnectClientAsync(ClientSocket clientSocket);
    public abstract void Close();
}