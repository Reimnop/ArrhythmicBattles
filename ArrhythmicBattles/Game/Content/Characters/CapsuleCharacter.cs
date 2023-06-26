﻿using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;

namespace ArrhythmicBattles.Game.Content.Characters;

public class CapsuleCharacter : Character
{
    public override string Name => "Capsule";
    public override AttributeList Attributes { get; } = new(
        new CharacterAttribute(AttributeType.JumpHeight, 20),
        new CharacterAttribute(AttributeType.AirSpeed, 15));

    public override CharacterInstance CreateInstance(IInputMethod inputMethod, ResourceManager resourceManager, PhysicsWorld physicsWorld)
    {
        return new CapsuleCharacterInstance(inputMethod, resourceManager, physicsWorld, this);
    }

    public override CharacterPreview CreatePreview(ResourceManager resourceManager)
    {
        return new CapsuleCharacterPreview(resourceManager);
    }
}
