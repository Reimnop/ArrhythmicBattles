using System.Text;

namespace ArrhythmicBattles.Networking.Packets;

public class TextPacket : Packet
{
    public string Text { get; set; } = string.Empty;

    public TextPacket()
    {
    }

    public TextPacket(string text)
    {
        Text = text;
    }

    public override Task<ReadOnlyMemory<byte>> SerializeAsync()
    {
        byte[] bytes = Encoding.UTF8.GetBytes(Text);
        return Task.FromResult((ReadOnlyMemory<byte>) bytes);
    }

    public override Task DeserializeAsync(ReadOnlyMemory<byte> buffer)
    {
        Text = Encoding.UTF8.GetString(buffer.ToArray());
        return Task.CompletedTask;
    }
}