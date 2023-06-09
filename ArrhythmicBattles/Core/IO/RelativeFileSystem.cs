namespace ArrhythmicBattles.Core.IO;

public class RelativeFileSystem : IFileSystem
{
    private readonly string basePath;
    
    public RelativeFileSystem(string basePath)
    {
        this.basePath = Path.GetFullPath(basePath);
    }
    
    public Stream Open(string path, FileMode mode)
    {
        var fullPath = Path.Combine(basePath, path);
        return File.Open(fullPath, mode);
    }

    public void CreateDirectory(string path)
    {
        var fullPath = Path.Combine(basePath, path);
        Directory.CreateDirectory(fullPath);
    }

    public void Delete(string path)
    {
        var fullPath = Path.Combine(basePath, path);
        File.Delete(fullPath);
    }

    public bool Exists(string path)
    {
        var fullPath = Path.Combine(basePath, path);
        return File.Exists(fullPath);
    }

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        var fullPath = Path.Combine(basePath, path);
        return Directory.EnumerateFiles(fullPath, searchPattern, searchOption);
    }

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        var fullPath = Path.Combine(basePath, path);
        return Directory.EnumerateDirectories(fullPath, searchPattern, searchOption);
    }
}