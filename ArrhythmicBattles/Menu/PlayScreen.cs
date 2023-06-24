using ArrhythmicBattles.Game;
using ArrhythmicBattles.Game.Content;
using ArrhythmicBattles.Game.Content.Characters;
using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Menu;

public class PlayScreen : IScreen
{
    public Node<ElementContainer> RootNode { get; }

    private readonly OrthographicCamera camera = new()
    {
        Position = Vector3.UnitZ * 500.0f,
        Size = 1.0f,
        DepthNear = 0.1f,
        DepthFar = 1000.0f
    };

    private CharacterPreview preview;
    
    private readonly MatrixStack matrixStack = new();
    private readonly GameLighting lighting = new();

    public PlayScreen(ABContext context, ScreenManager screenManager, ScopedInputProvider inputProvider)
    {
        preview = new CapsuleCharacterPreview(context.ResourceManager);
        
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/PlayButton.json")
                    {
                        Click = () => context.Engine.LoadScene(new GameScene(context))
                    })
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(16.0f, -80.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/BackButton.json") 
                    {
                        Click = () => screenManager.Switch(this, new MainScreen(context, screenManager, inputProvider))
                    })
                    .SetAnchor(Anchor.BottomLeft)
                    .SetEdges(-80.0f, 16.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new ViewportElement(context.Engine.Renderer, RenderPreview))
                    .SetAnchor(Anchor.FillRightEdge)
                    .SetEdges(96.0f, 16.0f, -336.0f, 16.0f))
        );
    }

    private void RenderPreview(Vector2i viewportSize, CommandList commandList)
    {
        commandList.UseClearColor(Color4.Transparent);
        commandList.UseLighting(lighting);
        
        var cameraData = camera.GetCameraData(viewportSize);
        var renderArgs = new RenderArgs(commandList, LayerType.Opaque, matrixStack, cameraData);
        preview.Render(renderArgs);
    }

    public void Update(UpdateArgs args)
    {
        RootNode.UpdateRecursively(args);
    }

    public void Render(RenderArgs args)
    {
        RootNode.RenderRecursively(args);
    }
}