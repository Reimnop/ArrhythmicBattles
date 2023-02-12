using ArrhythmicBattles.Networking.Server.Tcp;
using ArrhythmicBattles.Server;

using GameServer gameServer = new GameServer(new ServerTcpSocket(42069));
Task task = gameServer.Start();
task.Wait();

// Throw exception if task is faulted
if (task.IsFaulted)
{
    throw task.Exception!;
}