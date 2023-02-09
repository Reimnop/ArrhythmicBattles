﻿namespace ArrhythmicBattles.Networking.Packets;

public class PlayerLeavePacket : Packet
{
    public long Id { get; set; } = 0;

    public PlayerLeavePacket()
    {
    }
    
    public PlayerLeavePacket(long id)
    {
        Id = id;
    }
    
    public override Task<ReadOnlyMemory<byte>> SerializeAsync()
    {
        byte[] data = BitConverter.GetBytes(Id);
        return Task.FromResult((ReadOnlyMemory<byte>) data);
    }

    public override Task DeserializeAsync(ReadOnlyMemory<byte> buffer)
    {
        Id = BitConverter.ToInt64(buffer.Span);
        return Task.CompletedTask;
    }
}