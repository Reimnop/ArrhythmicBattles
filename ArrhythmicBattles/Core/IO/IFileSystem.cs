namespace ArrhythmicBattles.Core.IO;

/// <summary>
/// Represents a file system, virtual or physical.
/// </summary>
public interface IFileSystem
{
    Stream Open(string path, FileMode mode);
    string GetFullPath(string path);
    void CreateDirectory(string path);
    void Delete(string path);
    bool Exists(string path);
    IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
    IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);
}