using FlexFramework.Core.Rendering.Strategy;

namespace FlexFramework.Core.Rendering.Data;

public struct CustomDrawData : IDrawData
{
    public DrawFunc DrawFunc { get; }

    public CustomDrawData(DrawFunc drawFunc)
    {
        DrawFunc = drawFunc;
    }
}