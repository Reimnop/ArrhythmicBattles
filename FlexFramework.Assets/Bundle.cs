namespace FlexFramework.Assets;

/// <summary>
/// Stores a list of assets
/// </summary>
public class Bundle : IDisposable
{
    private readonly IAssetFactory assetFactory;

    private readonly List<string> assetNames = new List<string>();
    private readonly Dictionary<string, object> assets = new Dictionary<string, object>();
    private readonly Dictionary<string, AssetLocation> assetLocations = new Dictionary<string, AssetLocation>();

    private Stream? stream;

    public Bundle(IAssetFactory assetFactory)
    {
        this.assetFactory = assetFactory;
    }
    
    public void AddAsset(string name, object asset)
    {
        assets.Add(name, asset);
        assetNames.Add(name);
    }
    
    public void WriteTo(Stream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(assetNames.Count);
        foreach (string name in assetNames)
        {
            writer.Write(name); // write the name of the asset

            object asset = GetAsset(name);
            string typeId = assetFactory.GetAssetTypeId(asset.GetType());
            writer.Write(typeId); // write the type of the asset
            
            using MemoryStream memoryStream = new MemoryStream();
            assetFactory.WriteAsset(asset, memoryStream);

            writer.Write(memoryStream.Length); // write the compressed length of the asset
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.CopyTo(stream); // write the asset compressed
        }
    }
    
    public void ReadFrom(Stream stream)
    {
        this.stream = stream;
        
        BinaryReader reader = new BinaryReader(stream);
        int assetCount = reader.ReadInt32();
        for (int i = 0; i < assetCount; i++)
        {
            string name = reader.ReadString(); // read the name of the asset
            string typeId = reader.ReadString(); // read the type of the asset
            long length = reader.ReadInt64(); // read the compressed length of the asset
            
            AssetLocation assetLocation = new AssetLocation(typeId, stream.Position, length);
            assetLocations.Add(name, assetLocation);
            
            stream.Seek(length, SeekOrigin.Current); // skip the asset
        }
    }

    public object GetAsset(string name)
    {
        // if the asset is not loaded, load it
        if (stream != null && assetLocations.TryGetValue(name, out AssetLocation location))
        {
            SubStream subStream = new SubStream(stream, location.Offset, location.Size);
            object asset = assetFactory.ReadAsset(location.Type, subStream);
            
            assets.Add(name, asset);
            assetLocations.Remove(name);
            
            return asset;
        }

        // if the asset is loaded, return it
        return assets[name];
    }

    public T GetAsset<T>(string name)
    {
        return (T) GetAsset(name);
    }

    public void Dispose()
    {
        foreach (var (_, asset) in assets)
        {
            if (asset is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}