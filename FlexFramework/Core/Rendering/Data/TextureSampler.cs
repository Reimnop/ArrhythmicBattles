using FlexFramework.Core.Data;

namespace FlexFramework.Core.Rendering.Data;

public struct TextureSampler
{
    public ITextureView Texture { get; }
    public ISamplerView Sampler { get; }
    
    public TextureSampler(ITextureView texture, ISamplerView sampler)
    {
        Texture = texture;
        Sampler = sampler;
    }
}