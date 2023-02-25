using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UserInterface;

public abstract class Screen : IRenderable
{
    public abstract void Update(UpdateArgs args);
    public abstract void Render(RenderArgs args);
}