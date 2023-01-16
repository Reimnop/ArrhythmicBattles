namespace FlexFramework.Assets;

public interface IAssetFactory
{
    string GetAssetTypeId(Type type);
    void WriteAsset(object asset, Stream stream);
    object ReadAsset(string type, Stream stream);
}