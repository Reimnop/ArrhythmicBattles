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
    
    public static Hash128 operator ^(Hash128 left, Hash128 right)
    {
        Hash128 hash = new Hash128();
        for (int i = 0; i < 16; i++)
            hash.data[i] = (byte) (left.data[i] ^ right.data[i]);
        return hash;
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