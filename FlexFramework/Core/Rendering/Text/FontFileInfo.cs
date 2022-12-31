namespace FlexFramework.Core.Rendering.Text;

public struct FontFileInfo
{
    public string Name { get; }
    public int Size { get; }
    public string Path { get; }

    public FontFileInfo(string name, int size, string path)
    {
        Name = name;
        Size = size;
        Path = path;
    }
}