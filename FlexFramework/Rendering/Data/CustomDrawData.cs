using FlexFramework.Rendering.DefaultRenderingStrategies;

namespace FlexFramework.Rendering.Data;

public struct CustomDrawData : IDrawData
{
    public DrawFunc DrawFunc { get; }

    public CustomDrawData(DrawFunc drawFunc)
    {
        DrawFunc = drawFunc;
    }
}