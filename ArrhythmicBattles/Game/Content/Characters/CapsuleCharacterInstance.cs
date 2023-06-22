using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Resource;
using FlexFramework.Core;
using FlexFramework.Modelling;

namespace ArrhythmicBattles.Game.Content.Characters;

public class CapsuleCharacterInstance : CharacterInstance
{
    private readonly IInputMethod inputMethod;
    private readonly ResourceManager resourceManager;
    private readonly ModelEntity entity;

    public CapsuleCharacterInstance(IInputMethod inputMethod, ResourceManager resourceManager)
    {
        this.inputMethod = inputMethod;
        this.resourceManager = resourceManager;

        var model = resourceManager.Get<Model>("Models/Capsule");
        entity = new ModelEntity(model);
    }

    public override float GetAttributeMultiplier(AttributeType type) => 1.0f;

    public override void Update(UpdateArgs args)
    {
    }

    public override void Render(RenderArgs args)
    {
    }
}
