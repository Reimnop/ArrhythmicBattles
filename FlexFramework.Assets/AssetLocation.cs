namespace FlexFramework.Assets;

/// <summary>
/// Stores the location of an asset in an asset bundle
/// </summary>
public class AssetLocation
{
    public string Type { get; set; }
    public long Offset { get; set; }
    public long Size { get; set; }
    
    public AssetLocation(string type, long offset, long size)
    {
        Type = type;
        Offset = offset;
        Size = size;
    }
}