using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FlexFramework.Util;

namespace FlexFramework.Core.Data;

public class Buffer
{
    private struct ReadOnlyBuffer : IBufferView
    {
        public ReadOnlySpan<byte> Data => buffer.Data;
        public int Size => buffer.Size;
        public Hash128 Hash => buffer.Hash;
        
        private readonly Buffer buffer;
        
        public ReadOnlyBuffer(Buffer buffer)
        {
            this.buffer = buffer;
        }
    }
    
    public Span<byte> Data => new(data, 0, size);
    public int Size => size;
    public Hash128 Hash => hash;
    
    private byte[] data;
    private int size = 0;
    private Hash128 hash;
    
    public Buffer(int capacity = 1024)
    {
        data = new byte[capacity];
        UpdateHash();
    }

    private void UpdateHash()
    { 
        hash = HashUtil.GetMD5(data);
    }

    public void SetData(IntPtr ptr, int length)
    {
        if (length > data.Length)
        {
            data = new byte[length];
        }
        
        Marshal.Copy(ptr, data, 0, length);
        size = length;
        
        UpdateHash();
    }
    
    public unsafe void SetData<T>(ReadOnlySpan<T> buffer) where T : unmanaged
    {
        var length = buffer.Length * Unsafe.SizeOf<T>();
        
        fixed (T* ptr = buffer)
        {
            SetData((IntPtr) ptr, length);
        }
    }
    
    public void Clear()
    {
        size = 0;
        UpdateHash();
    }
    
    public IBufferView AsReadOnly()
    {
        return new ReadOnlyBuffer(this);
    }
}