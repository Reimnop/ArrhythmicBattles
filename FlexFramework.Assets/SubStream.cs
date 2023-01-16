namespace FlexFramework.Assets;

/// <summary>
/// A thread-safe wrapper for <see cref="System.IO.Stream"/> that allows for multiple readers and writers
/// </summary>
public class SubStream : Stream
{
    public override bool CanRead { get; }
    public override bool CanSeek => true;
    public override bool CanWrite { get; }
    public override long Length { get; }
    public override long Position { get; set; } = 0;
    
    private readonly Stream stream;
    private readonly long offset;

    public SubStream(Stream stream, long offset, long length)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanSeek)
        {
            throw new ArgumentException("Stream must be seekable", nameof(stream));
        }
        
        CanRead = stream.CanRead;
        CanWrite = stream.CanWrite;

        this.stream = stream;
        this.offset = offset;
        Length = length;
    }
    
    public override void Flush()
    {
        lock (stream)
        {
            stream.Flush();
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!CanRead)
        {
            throw new NotSupportedException("Stream does not support reading");
        }
        
        lock (stream)
        {
            stream.Seek(this.offset + Position, SeekOrigin.Begin);
            stream.Read(buffer, offset, count);
        
            Position += count;
            return count;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                Position = offset;
                break;
            case SeekOrigin.Current:
                Position += offset;
                break;
            case SeekOrigin.End:
                Position = Length - offset;
                break;
        }

        return Position;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException("Stream does not support setting length");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (!CanWrite)
        {
            throw new NotSupportedException("Stream does not support writing");
        }
        
        lock (stream)
        {
            stream.Seek(this.offset + Position, SeekOrigin.Begin);
            stream.Write(buffer, offset, count);
            
            Position += count;
        }
    }
}