using FlexFramework.Core.Rendering;
using FlexFramework.Core;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Drawables;

public abstract class Drawable : IRenderable
{
    public Bounds Bounds { get; set; }
    public Transform Transform { get; set; } = new Transform(Vector2.Zero, Vector2.One, 0.0f);

    public abstract void Render(RenderArgs args);
}