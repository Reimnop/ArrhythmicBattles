using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using ArrhythmicBattles.Common;
using ArrhythmicBattles.Networking;
using ArrhythmicBattles.Networking.Client;
using ArrhythmicBattles.Networking.Packets;

namespace NetworkTest;

class Program
{
    public static void Main(string[] args)
    {
        Client().GetAwaiter().GetResult();
    }

    private static async Task Client()
    {
        long id = 0;
        
        GameClient client = new TcpGameClient(IPAddress.Loopback, 42069);
        NetworkHandler networkHandler = new NetworkHandler(client);

        Console.WriteLine("Sending auth packet");
        await networkHandler.SendPacketAsync(new AuthPacket($"TestUser-{id}", id));
        Console.WriteLine("Auth packet sent, waiting for packets");

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        long lastPacketReceivedMs = 0;

        networkHandler.PacketReceived.AddCallback((sender, packet) =>
        {
            lastPacketReceivedMs = stopwatch.ElapsedMilliseconds;
            return Task.CompletedTask;
        });
        
        Dictionary<long, string> playerMap = new Dictionary<long, string>();

        while (true)
        {
            Packet? packet = await networkHandler.ReceivePacketAsync();
            
            if (packet is null)
            {
                Console.WriteLine("Received null packet, disconnecting");
                break;
            }

            PlayerLeavePacket? playerLeavePacket = packet as PlayerLeavePacket;
            PlayerListPacket? playerListPacket = packet as PlayerListPacket;
            PlayerJoinPacket? playerJoinPacket = packet as PlayerJoinPacket;

            if (packet is HeartbeatPacket)
            {
                Console.WriteLine("Received heartbeat");
            }

            if (playerLeavePacket != null)
            {
                Console.WriteLine($"Player '{playerMap[playerLeavePacket.Id]}' left the game");
            }

            if (playerListPacket != null)
            {
                Console.WriteLine("Received player list:");
                
                playerMap.Clear();
                foreach (PlayerProfile playerProfile in playerListPacket.Players)
                {
                    playerMap.Add(playerProfile.Id, playerProfile.Username);
                    Console.WriteLine($"    - {playerProfile.Username} (ID: {playerProfile.Id})");
                }
            }
            
            if (playerJoinPacket != null)
            {
                Console.WriteLine($"Player '{playerMap[playerJoinPacket.Id]}' joined the game");
            }

            await Task.Delay(5);

            Exception? exception = networkHandler.GetException();
            if (exception != null)
            {
                Console.WriteLine($"Encountered error: {exception}");
                break;
            }

            if (stopwatch.ElapsedMilliseconds - lastPacketReceivedMs >= 20000)
            {
                Console.WriteLine("Last packet was over 20 seconds ago, disconnecting");
                break;
            }
        }
        
        stopwatch.Stop();
        networkHandler.Dispose();
        client.Close();
    }
}