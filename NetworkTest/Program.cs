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
        long id = DateTime.Now.ToBinary();
        
        GameClient client = new TcpGameClient(IPAddress.Loopback, 42069);
        TypedPacketTunnel tunnel = new TypedPacketTunnel(client);
        await tunnel.SendAsync(new AuthPacket($"TestUser-{id}", id));

        while (true)
        {
            Packet packet = await tunnel.ReceiveAsync();
            
            if (packet is PlayerListPacket playerListPacket)
            {
                Console.WriteLine("Received player list:");
                foreach (PlayerProfile profile in playerListPacket.Players)
                {
                    Console.WriteLine($"- {profile.Username} ({profile.Id})");
                }
            }
            
            if (packet is PlayerJoinPacket playerJoinPacket)
            {
                Console.WriteLine($"Player with id '{playerJoinPacket.Id}' joined the game");
            }
            
            if (packet is PlayerLeavePacket playerLeavePacket)
            {
                Console.WriteLine($"Player with id '{playerLeavePacket.Id}' left the game");
            }
        }
    }
}