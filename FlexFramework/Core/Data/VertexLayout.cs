using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FlexFramework.Core.Data;

public enum VertexAttributeType
{
    Byte,
    UByte,
    Short,
    UShort,
    Int,
    UInt,
    Float,
    Double
}

public enum VertexAttributeIntent
{
    Position,
    Normal,
    Tangent,
    Color,
    TexCoord,
    BoneWeight,
    BoneIndex,
    Any
}

public struct VertexAttribute
{
    public VertexAttributeIntent Intent { get; set; }
    public VertexAttributeType Type { get; set; }
    public int Size { get; set; }
    public int Offset { get; set; }
    
    public VertexAttribute(VertexAttributeIntent intent, VertexAttributeType type, int size, int offset)
    {
        Intent = intent;
        Type = type;
        Size = size;
        Offset = offset;
    }
}

public class VertexAttributeAttribute : Attribute
{
    public VertexAttributeIntent Intent { get; set; }
    public VertexAttributeType Type { get; set; }
    public int Size { get; set; }
    
    public VertexAttributeAttribute(VertexAttributeIntent intent, VertexAttributeType type, int size)
    {
        Intent = intent;
        Type = type;
        Size = size;
    }
}

public class VertexLayout
{
    public int Stride { get; }
    public ReadOnlySpan<VertexAttribute> Attributes => attributes.AsSpan();

    private readonly VertexAttribute[] attributes;

    public VertexLayout(int stride, params VertexAttribute[] attributes)
    {
        Stride = stride;
        this.attributes = attributes;
    }
    
    public VertexLayout DeepCopy()
    {
        return new VertexLayout(Stride, attributes.ToArray());
    }

    public static VertexLayout GetLayout<T>() where T : unmanaged
    {
        var type = typeof(T);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var attributes = new List<VertexAttribute>();
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<VertexAttributeAttribute>();
            if (attribute == null)
                continue;
            var offset = Marshal.OffsetOf(type, field.Name).ToInt32();
            attributes.Add(new VertexAttribute(attribute.Intent, attribute.Type, attribute.Size, offset));
        }
        return new VertexLayout(Unsafe.SizeOf<T>(), attributes.ToArray());
    }
}