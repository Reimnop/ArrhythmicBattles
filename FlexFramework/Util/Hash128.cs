namespace FlexFramework.Util;

public unsafe struct Hash128
{
    private fixed byte data[16];

    public Hash128(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length != 16)
            throw new ArgumentException("Buffer must be 16 bytes long!", nameof(buffer));

        fixed (byte* ptr = data)
        {
            buffer.CopyTo(new Span<byte>(ptr, 16));
        }
    }

    public byte this[int index]
    {
        get
        {
            if (index < 0 || index >= 16)
                throw new IndexOutOfRangeException(nameof(index));
            
            return data[index];
        }
        set
        {
            if (index < 0 || index >= 16)
                throw new IndexOutOfRangeException(nameof(index));
            
            data[index] = value;
        }
    }
}