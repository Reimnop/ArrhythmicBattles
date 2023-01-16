using System.IO.Compression;
using FlexFramework.Assets;

namespace AssetPacker;

public class AssetFactory : IAssetFactory
{
    public string GetAssetTypeId(Type type)
    {
        return type.Name;
    }

    public void WriteAsset(object asset, Stream stream)
    {
        using DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Compress, true);
        BinaryWriter writer = new BinaryWriter(deflateStream);

        if (asset is IBinarySerializable serializable)
        {
            serializable.Serialize(writer);
            return;
        }

        throw new NotSupportedException();
    }

    public object ReadAsset(string type, Stream stream)
    {
        throw new NotImplementedException();
    }
}