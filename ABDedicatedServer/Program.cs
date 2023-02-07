using ArrhythmicBattles.Networking.Server.Tcp;
using ArrhythmicBattles.Server;

using GameServer gameServer = new GameServer(new ServerTcpSocket(42069));
gameServer.Start().GetAwaiter().GetResult();