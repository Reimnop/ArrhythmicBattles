using FlexFramework.Core.Rendering.DefaultRenderingStrategies;

namespace FlexFramework.Core.Rendering.Data;

public struct CustomDrawData : IDrawData
{
    public DrawFunc DrawFunc { get; }

    public CustomDrawData(DrawFunc drawFunc)
    {
        DrawFunc = drawFunc;
    }
}