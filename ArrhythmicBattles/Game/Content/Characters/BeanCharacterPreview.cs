using ArrhythmicBattles.Core.Resource;
using FlexFramework.Core;
using FlexFramework.Modelling;

namespace ArrhythmicBattles.Game.Content.Characters;

public class BeanCharacterPreview : CharacterPreview
{
    private readonly ModelEntity entity;

    public BeanCharacterPreview(ResourceManager resourceManager)
    {
        var model = resourceManager.Get<Model>("Models/Capsule.dae");
        entity = new ModelEntity(model);
    }

    public override void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Scale(0.4f, 0.4f, 0.4f);
        entity.Render(args);
        matrixStack.Pop();
    }
}
