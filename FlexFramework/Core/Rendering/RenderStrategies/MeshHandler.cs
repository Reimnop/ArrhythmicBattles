﻿using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using Buffer = FlexFramework.Core.Rendering.Data.Buffer;
using Timer = FlexFramework.Util.Timer;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public class GpuMesh : IDisposable
{
    public VertexArray VertexArray { get; }
    public Buffer VertexBuffer { get; }
    public Buffer? IndexBuffer { get; }
    
    public GpuMesh(VertexArray vertexArray, Buffer vertexBuffer, Buffer? indexBuffer)
    {
        VertexArray = vertexArray;
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
    }
    
    public void Dispose()
    {
        VertexArray.Dispose();
        VertexBuffer.Dispose();
        IndexBuffer?.Dispose();
    }
}

public class MeshHandler
{
    private readonly IDictionary<VertexAttributeIntent, int> attributeLocations;
    private readonly GarbageCollector<IMeshView, GpuMesh> gc;
    private readonly Timer timer;

    public MeshHandler(params (VertexAttributeIntent, int)[] locations)
    {
        attributeLocations = locations.ToDictionary(x => x.Item1, x => x.Item2);
        gc = new GarbageCollector<IMeshView, GpuMesh>(GetHash, CreateMesh);
        timer = new Timer(1.0f, () => gc.Sweep());
    }

    public void Update(float deltaTime)
    {
        timer.Update(deltaTime);
    }

    public GpuMesh GetMesh(IMeshView mesh)
    {
        return gc.GetOrAllocate(mesh);
    }
    
    private static Hash256 GetHash(IMeshView mesh)
    {
        Hash256 hash = mesh.VertexBuffer.Hash;
        hash ^= mesh.VertexLayout.Hash;
        if (mesh.IndexBuffer != null)
            hash ^= mesh.IndexBuffer.Hash;
        return hash;
    }

    private GpuMesh CreateMesh(IMeshView mesh)
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

        return new GpuMesh(vertexArray, vertexBuffer, indexBuffer);
    }
}