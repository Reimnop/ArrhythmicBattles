namespace FlexFramework.Core.Data;

public class TextureSampler
{
    public Texture Texture { get; }
    public Sampler Sampler { get; }
    
    public TextureSampler(Texture texture, Sampler sampler)
    {
        Texture = texture;
        Sampler = sampler;
    }
}