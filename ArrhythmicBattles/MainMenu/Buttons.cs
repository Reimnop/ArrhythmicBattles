﻿using ArrhythmicBattles.Settings;
using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Util;
using FlexFramework.Rendering;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.MainMenu;

public class Buttons : Entity, IRenderable
{
    public Vector2i Position
    {
        get => stackLayout.Position;
        set => stackLayout.Position = value;
    }

    private readonly VerticalStackLayout stackLayout;
    private readonly KeyboardNavigator navigator;
    private readonly EntityGroup entityGroup;

    public Buttons(FlexFrameworkMain engine, MainMenuScene scene, Vector2i buttonSize)
    {
        entityGroup = new EntityGroup();

        ButtonEntity singleplayerButton = new ButtonEntity(engine)
            .WithText("SINGLEPLAYER")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(buttonSize);

        ButtonEntity multiplayerButton = new ButtonEntity(engine)
            .WithText("MULTIPLAYER")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(buttonSize);
        
        ButtonEntity settingsButton = new ButtonEntity(engine)
            .WithText("SETTINGS")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(buttonSize)
            .AddPressedCallback(() => scene.LoadSettingsScene());

        ButtonEntity exitButton = new ButtonEntity(engine)
            .WithText("EXIT")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextUnfocusedColor(new Color4(233, 81, 83, 255))
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(buttonSize)
            .AddPressedCallback(() => engine.Close());

        stackLayout = new VerticalStackLayout(engine)
            .WithPosition(48, 304);

        stackLayout.AddChild(singleplayerButton);
        stackLayout.AddChild(multiplayerButton);
        stackLayout.AddChild(settingsButton);
        stackLayout.AddChild(exitButton);
        
        entityGroup.AddEntity(singleplayerButton, multiplayerButton, settingsButton, exitButton);
        
        NavNode playNode = new NavNode(singleplayerButton);
        NavNode multiplayerNode = new NavNode(multiplayerButton);
        NavNode configNode = new NavNode(settingsButton);
        NavNode exitNode = new NavNode(exitButton);

        playNode.Top = exitNode;
        playNode.Bottom = multiplayerNode;
        multiplayerNode.Top = playNode;
        multiplayerNode.Bottom = configNode;
        configNode.Top = multiplayerNode;
        configNode.Bottom = exitNode;
        exitNode.Top = configNode;
        exitNode.Bottom = playNode;

        navigator = new KeyboardNavigator(engine, playNode);
    }
    
    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        stackLayout.Update(args);
        navigator.Update(args);
        entityGroup.Update(args);
    }
    
    public void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        navigator.Render(renderer, layerId, matrixStack, cameraData);
        entityGroup.Render(renderer, layerId, matrixStack, cameraData);
    }

    public override void Dispose()
    {
        entityGroup.Dispose();
    }
}