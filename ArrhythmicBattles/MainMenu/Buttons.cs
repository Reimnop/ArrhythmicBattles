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
    private readonly VerticalStackLayout stackLayout;
    private readonly KeyboardNavigator navigator;
    private readonly EntityGroup entityGroup;

    public Buttons(FlexFrameworkMain engine)
    {
        entityGroup = new EntityGroup();
        
        ButtonEntity playButton = new ButtonEntity(engine)
            .WithText("PLAY")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(768, 56);
        
        ButtonEntity multiplayerButton = new ButtonEntity(engine)
            .WithText("MULTIPLAYER")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(768, 56);
        
        ButtonEntity configButton = new ButtonEntity(engine)
            .WithText("CONFIG")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(768, 56);

        ButtonEntity exitButton = new ButtonEntity(engine)
            .WithText("EXIT")
            .WithOrigin(0.0, 1.0)
            .WithTextPosOffset(10, 36)
            .WithTextUnfocusedColor(new Color4(233, 81, 83, 255))
            .WithTextFocusedColor(new Color4(33, 33, 33, 255))
            .WithSize(768, 56)
            .AddPressedCallback(() => engine.Close());

        stackLayout = new VerticalStackLayout(engine)
            .WithPosition(48, 304)
            .WithSize(768, 0);
        
        stackLayout.AddChild(playButton);
        stackLayout.AddChild(multiplayerButton);
        stackLayout.AddChild(configButton);
        stackLayout.AddChild(exitButton);
        
        entityGroup.AddEntity(playButton, multiplayerButton, configButton, exitButton);
        
        NavNode playNode = new NavNode(playButton);
        NavNode multiplayerNode = new NavNode(multiplayerButton);
        NavNode configNode = new NavNode(configButton);
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