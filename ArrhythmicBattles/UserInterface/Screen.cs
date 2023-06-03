using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;

namespace ArrhythmicBattles.UserInterface;

public abstract class Screen : IRenderable
{
    public abstract Node<ElementContainer>? RootNode { get; }

    public abstract void Update(UpdateArgs args);
    public abstract void Render(RenderArgs args);
}