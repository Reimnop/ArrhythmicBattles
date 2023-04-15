using System.Security.Cryptography;

namespace FlexFramework.Util;

public static class HashUtil
{
    public static Hash128 GetMD5(ReadOnlySpan<byte> buffer)
    {
        Span<byte> hash = stackalloc byte[16];
        if (MD5.TryHashData(buffer, hash, out _))
        {
            return new Hash128(hash);
        }
        
        throw new Exception("Failed to hash data with MD5!");
    }
}