using ArrhythmicBattles.Core.Input;
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
    private class InputMethodAdapter : INamed
    {
        public string Name { get; }
        public IInputMethod InputMethod { get; }
        
        public InputMethodAdapter(string name, IInputMethod inputMethod)
        {
            Name = name;
            InputMethod = inputMethod;
        }
    }
    
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
    private InputMethodAdapter inputMethod;
    
    private readonly MySelectorElement<Character> characterSelector;
    private readonly MySelectorElement<InputMethodAdapter> inputMethodSelector;
    private readonly MatrixStack matrixStack = new();
    private readonly GameLighting lighting = new();

    public PlayScreen(ABContext context, ScreenManager screenManager, ScopedInputProvider inputProvider)
    {
        var engine = context.Engine;
        var characterRegistry = context.CharacterRegistry;
        
        // Get all input methods
        var inputMethods = Enumerable.Empty<InputMethodAdapter>()
            .Append(new InputMethodAdapter("Keyboard", new KeyboardInputMethod(engine.KeyboardState)))
            .Concat(engine.JoystickStates
                .Where(x => x != null)
                .Select((x, i) => new InputMethodAdapter($"[{i}] {x.Name}", new JoystickInputMethod(x))));
        
        RootNode = screenManager.BuildInterface(
            new InterfaceTreeBuilder()
                .SetAnchor(Anchor.Fill)
                .AddChild(new InterfaceTreeBuilder()
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/PlayButton.json") // Play button
                    {
                        Click = () => context.Engine.LoadScene(() => new GameScene(context, inputMethod.InputMethod, character))
                    })
                    .SetAnchor(Anchor.TopLeft)
                    .SetEdges(16.0f, -80.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder() // Back button
                    .SetElement(new MyButtonElement(inputProvider, context.ResourceManager, "Styles/BackButton.json") 
                    {
                        Click = () => screenManager.Switch(this, new MainScreen(context, screenManager, inputProvider))
                    })
                    .SetAnchor(Anchor.BottomLeft)
                    .SetEdges(-80.0f, 16.0f, 16.0f, -336.0f))
                .AddChild(new InterfaceTreeBuilder() // Character preview
                    .SetElement(new ViewportElement(context.Engine.Renderer, RenderPreview))
                    .SetAnchor(Anchor.FillRightEdge)
                    .SetEdges(176.0f, 16.0f, -400.0f, 16.0f))
                .AddChild(new InterfaceTreeBuilder() // Input method selector
                    .SetElement(inputMethodSelector = new MySelectorElement<InputMethodAdapter>(
                        inputMethods, 
                        inputProvider, 
                        context.ResourceManager, 
                        "Styles/Selector.json"))
                    .SetAnchor(Anchor.TopRight)
                    .SetEdges(16.0f, -80.0f, -400.0f, 16.0f))
                .AddChild(new InterfaceTreeBuilder() // Character selector
                    .SetElement(characterSelector = new MySelectorElement<Character>(
                        characterRegistry, 
                        inputProvider, 
                        context.ResourceManager, 
                        "Styles/Selector.json"))
                    .SetAnchor(Anchor.TopRight)
                    .SetEdges(96.0f, -160.0f, -400.0f, 16.0f))
        );

        // Load the first character
        character = characterSelector.Selected;
        preview = character.CreatePreview(context.ResourceManager);
        characterSelector.SelectionChanged += newCharacter =>
        {
            if (preview is IDisposable disposable)
                disposable.Dispose();
            
            character = newCharacter;
            preview = character.CreatePreview(context.ResourceManager);
        };
        
        // Load the first input method
        inputMethod = inputMethodSelector.Selected;
        inputMethodSelector.SelectionChanged += newInputMethod => inputMethod = newInputMethod;
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