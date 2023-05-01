using System.Text;

namespace FlexFramework.Util;

public unsafe struct Hash256
{
    const int Length = 32;
    
    private fixed byte data[Length];

    public Hash256(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length != Length)
            throw new ArgumentException($"Buffer must be {Length} bytes long!", nameof(buffer));

        fixed (byte* ptr = data)
        {
            buffer.CopyTo(new Span<byte>(ptr, Length));
        }
    }
    
    public static Hash256 operator ^(Hash256 left, Hash256 right)
    {
        Span<byte> result = stackalloc byte[Length];
        for (int i = 0; i < Length; i++)
        {
            result[i] = (byte)(left[i] ^ right[i]);
        }
        return new Hash256(result);
    }

    public override string ToString()
    {
        // Return hex string
        StringBuilder sb = new StringBuilder(Length * 2);
        for (int i = 0; i < Length; i++)
        {
            sb.Append(data[i].ToString("x2"));
        }
        return sb.ToString();
    }

    public byte this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException(nameof(index));
            
            return data[index];
        }
        set
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException(nameof(index));
            
            data[index] = value;
        }
    }
}