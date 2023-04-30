using System.Diagnostics;
using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using Buffer = FlexFramework.Core.Rendering.Data.Buffer;

namespace FlexFramework.Core.Rendering.RenderStrategies;

// TODO: This is dumb. Don't use this in production.
// Someone should probably implement a GC for this. A simple one would be fine.
public class MeshHandler
{
    private readonly IDictionary<VertexAttributeIntent, int> attributeLocations;

    public MeshHandler(params (VertexAttributeIntent, int)[] locations)
    {
        attributeLocations = locations.ToDictionary(x => x.Item1, x => x.Item2);
    }

    public (VertexArray, Buffer, Buffer?) GetMesh(IMeshView mesh)
    {
        Buffer vertexBuffer = new Buffer("vertex");
        vertexBuffer.LoadData(mesh.VertexBuffer.Data);
        
        Buffer? indexBuffer = null;
        if (mesh.IndexBuffer != null)
        {
            indexBuffer = new Buffer("index");
            indexBuffer.LoadData(mesh.IndexBuffer.Data);
        }
        
        VertexArray vertexArray = new VertexArray("mesh");
        if (indexBuffer != null)
        {
            vertexArray.ElementBuffer(indexBuffer);
        }

        foreach (var attribute in mesh.VertexLayout.Attributes)
        {
            if (!attributeLocations.TryGetValue(attribute.Intent, out int location))
            {
                continue;
            }

            // imagine becoming yandere dev
            // can't be me
            if (attribute.Type == VertexAttributeType.Byte)
                vertexArray.VertexBuffer(vertexBuffer, location, location, attribute.Size, attribute.Offset, VertexAttribIntegerType.Byte, mesh.VertexSize);
            else if (attribute.Type == VertexAttributeType.UByte)
                vertexArray.VertexBuffer(vertexBuffer, location, location, attribute.Size, attribute.Offset, VertexAttribIntegerType.UnsignedByte, mesh.VertexSize);
            else if (attribute.Type == VertexAttributeType.Short)
                vertexArray.VertexBuffer(vertexBuffer, location, location, attribute.Size, attribute.Offset, VertexAttribIntegerType.Short, mesh.VertexSize);
            else if (attribute.Type == VertexAttributeType.UShort)
                vertexArray.VertexBuffer(vertexBuffer, location, location, attribute.Size, attribute.Offset, VertexAttribIntegerType.UnsignedShort, mesh.VertexSize);
            else if (attribute.Type == VertexAttributeType.Int)
                vertexArray.VertexBuffer(vertexBuffer, location, location, attribute.Size, attribute.Offset, VertexAttribIntegerType.Int, mesh.VertexSize);
            else if (attribute.Type == VertexAttributeType.UInt)
                vertexArray.VertexBuffer(vertexBuffer, location, location, attribute.Size, attribute.Offset, VertexAttribIntegerType.UnsignedInt, mesh.VertexSize);
            else if (attribute.Type == VertexAttributeType.Float)
                vertexArray.VertexBuffer(vertexBuffer, location, location, attribute.Size, attribute.Offset, VertexAttribType.Float, false, mesh.VertexSize);
            else if (attribute.Type == VertexAttributeType.Double)
                vertexArray.VertexBuffer(vertexBuffer, location, location, attribute.Size, attribute.Offset, VertexAttribType.Double, false, mesh.VertexSize);
        }

        return (vertexArray, vertexBuffer, indexBuffer);
    }
}