using FlexFramework.Core;
using FlexFramework.Core.UserInterface;
using FlexFramework.Util;

namespace ArrhythmicBattles.UserInterface;

public interface IScreen : IRenderable
{
    Node<ElementContainer>? RootNode { get; }

    void Update(UpdateArgs args);
}