using ArrhythmicBattles.Common;
using ArrhythmicBattles.Networking.Packets;

namespace ArrhythmicBattles.Networking;

public class TypedPacketTunnel
{
    private readonly Map<Identifier, Type> packetTypes = new Map<Identifier, Type>()
    {
        { new Identifier("arrhythmicbattles", "auth"), typeof(AuthPacket) },
        { new Identifier("arrhythmicbattles", "heartbeat"), typeof(HeartbeatPacket) },
        { new Identifier("arrhythmicbattles", "player_list"), typeof(PlayerListPacket) },
        { new Identifier("arrhythmicbattles", "player_join"), typeof(PlayerJoinPacket) },
        { new Identifier("arrhythmicbattles", "player_leave"), typeof(PlayerLeavePacket) }
    };

    private readonly Dictionary<Type, Func<Packet>> packetFactories = new Dictionary<Type, Func<Packet>>()
    {
        { typeof(AuthPacket), () => new AuthPacket() },
        { typeof(HeartbeatPacket), () => new HeartbeatPacket() },
        { typeof(PlayerListPacket), () => new PlayerListPacket() },
        { typeof(PlayerJoinPacket), () => new PlayerJoinPacket() },
        { typeof(PlayerLeavePacket), () => new PlayerLeavePacket() }
    };

    private readonly ISenderReceiver senderReceiver;

    public TypedPacketTunnel(ISenderReceiver senderReceiver)
    {
        this.senderReceiver = senderReceiver;
    }

    public async Task SendAsync(Packet packet, CancellationToken cancellationToken = default)
    {
        // Serialize the packet
        byte[] packetBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(packetTypes.Reverse[packet.GetType()]);
        
            ReadOnlyMemory<byte> buffer = await packet.SerializeAsync();
            writer.Write(buffer.Span);
            
            packetBytes = stream.ToArray();
        }
        
        // Write packet length
        byte[] sendBuffer;
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(packetBytes.Length);
            writer.Write(packetBytes);
            sendBuffer = stream.ToArray();
        }

        await senderReceiver.SendAsync(sendBuffer, cancellationToken);
    }
    
    public async Task<Packet?> ReceiveAsync()
    {
        ReadOnlyMemory<byte> lengthBuffer = await senderReceiver.ReceiveAsync(4);
        if (lengthBuffer.Length == 0)
        {
            return null;
        } 
        
        int packetLength = BitConverter.ToInt32(lengthBuffer.Span);
        ReadOnlyMemory<byte> packetBuffer = await senderReceiver.ReceiveAsync(packetLength);
        if (packetBuffer.Length == 0)
        {
            return null;
        }

        using MemoryStream stream = new MemoryStream(packetBuffer.ToArray());
        BinaryReader reader = new BinaryReader(stream);
        Identifier packetIdentifier = reader.ReadString();
        
        if (!packetTypes.Forward.Contains(packetIdentifier))
        {
            throw new InvalidDataException($"Invalid packet identifier '{packetIdentifier}'");
        }

        Type packetType = packetTypes.Forward[packetIdentifier];
        
        if (!packetFactories.TryGetValue(packetType, out Func<Packet>? packetFactory))
        {
            throw new InvalidDataException($"Cannot create packet type '{packetType}'");
        }

        Packet packet = packetFactory();

        // Slice the packet buffer to exclude the packet identifier
        ReadOnlyMemory<byte> packetData = packetBuffer.Slice((int) stream.Position);
        await packet.DeserializeAsync(packetData);
        return packet;
    }
}