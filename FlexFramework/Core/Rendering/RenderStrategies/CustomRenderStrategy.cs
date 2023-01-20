using FlexFramework.Core.Rendering.Data;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public delegate void DrawFunc(GLStateManager glStateManager);

public class CustomRenderStrategy : RenderStrategy
{
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        CustomDrawData customDrawData = EnsureDrawDataType<CustomDrawData>(drawData);
        customDrawData.DrawFunc(glStateManager);
    }
}