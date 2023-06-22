using OpenTK.Mathematics;
using FlexFramework.Core;

namespace ArrhythmicBattles.Game.Content;

public abstract class CharacterInstance : IUpdateable, IRenderable
{
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Quaternion Rotation { get; set; } = Quaternion.Identity;

    public abstract float GetAttributeMultiplier(AttributeType type);

    public abstract void Update(UpdateArgs args);
    public abstract void Render(RenderArgs args);
}
