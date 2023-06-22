using ArrhythmicBattles.Core.Resource;
using FlexFramework.Core;
using FlexFramework.Modelling;

namespace ArrhythmicBattles.Game.Content.Characters;

public class CapsuleCharacterPreview : CharacterPreview
{
    private readonly ModelEntity entity;

    public CapsuleCharacterPreview(ResourceManager resourceManager)
    {
        var model = resourceManager.Get<Model>("Models/Capsule.dae");
        entity = new ModelEntity(model);
    }

    public override void Render(RenderArgs args)
    {
        entity.Render(args);
    }
}
