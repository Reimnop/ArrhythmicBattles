namespace FlexFramework.Core.Data;

public class Texture : DataObject
{
    public int Width { get; }
    public int Height { get; }
    
    public Texture(string name, int width, int height) : base(name)
    {
        Width = width;
        Height = height;
    }
}