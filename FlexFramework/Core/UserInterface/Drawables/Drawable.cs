using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface.Animators;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Drawables;

public abstract class Drawable : IRenderable
{
    public Bounds Bounds { get; }
    public Animator? Animator { get; }
    public Transform Transform { get; set; }

    public Drawable(Bounds bounds, Animator? animator)
    {
        Bounds = bounds;
        Animator = animator;
        Transform = new Transform(Vector2.Zero, Vector2.One, 0.0f);
    }

    public virtual void Update(float deltaTime)
    {
        Animator?.Update(this, deltaTime);
    }
    
    public abstract void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData);
}