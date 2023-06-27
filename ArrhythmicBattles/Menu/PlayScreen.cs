using ArrhythmicBattles.Game;
using ArrhythmicBattles.Game.Content;
using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Menu;

public class PlayScreen : IScreen, IDisposable
{
    public Node<ElementContainer> RootNode { get; }

    private readonly OrthographicCamera camera = new()
    {
        Position = Vector3.UnitZ * 500.0f,
        Size = 1.0f,
        DepthNear = 0.1f,
        DepthFar = 1000.0f
    };

    private Character character;
    private CharacterPreview preview;
    
    private readonly MySelectorElement<Character> characterSelector;
    private readonly MatrixStack matrixStack = new();
    private readonly GameLighting lighting = new();

    public PlayScreen(ABContext context, ScreenManager screenManager, ScopedInputProvider inputProvider)
    {
        var characterRegistry = context.CharacterRegistry;
        
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/PlayButton.json")
                    {
                        Click = () => context.Engine.LoadScene(() => new GameScene(context, character))
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
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(characterSelector = new MySelectorElement<Character>(
                        characterRegistry, 
                        inputProvider, 
                        context.ResourceManager, 
                        "Styles/Selector.json"))
                    .SetAnchor(Anchor.TopRight)
                    .SetEdges(16.0f, -80.0f, -336.0f, 16.0f))
        );

        character = characterSelector.Selected;
        preview = character.CreatePreview(context.ResourceManager);
        
        characterSelector.SelectionChanged += newCharacter =>
        {
            if (preview is IDisposable disposable)
                disposable.Dispose();
            
            character = newCharacter;
            preview = character.CreatePreview(context.ResourceManager);
        };
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

    public void Dispose()
    {
        if (preview is IDisposable disposable)
            disposable.Dispose();
    }
}