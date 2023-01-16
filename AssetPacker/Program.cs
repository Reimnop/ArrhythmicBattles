using AssetPacker;
using FlexFramework.Assets;

AssetFactory factory = new AssetFactory();

DirectoryInfo assetsDir = new DirectoryInfo("Assets");
foreach (DirectoryInfo directory in assetsDir.EnumerateDirectories())
{
    Bundle bundle = new Bundle(factory);
    foreach (FileInfo fileInfo in directory.EnumerateFiles())
    {
        string extension = fileInfo.Extension.ToLower();
        string assetName = Path.GetFileNameWithoutExtension(fileInfo.Name);
        switch (extension)
        {
            case ".png": 
                bundle.AddAsset(assetName, new Texture2D(fileInfo.FullName));
                break;
            default:
                Console.WriteLine($"Warning: Unsupported file type [{fileInfo.Name}] at [{fileInfo.FullName}] (file ignored)");
                break;
        }
    }

    string bundleName = $"{directory.Name}.bin";
    string bundlePath = Path.Combine("Output", bundleName);
    Directory.CreateDirectory(Path.GetDirectoryName(bundlePath)!);

    using FileStream stream = File.Create(bundlePath);
    bundle.WriteTo(stream);
    
    Console.WriteLine($"Bundle [{bundleName}] written to [{bundlePath}]");
}

