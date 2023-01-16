namespace AssetPacker;

public interface IBinarySerializable
{
    void Serialize(BinaryWriter writer);
}